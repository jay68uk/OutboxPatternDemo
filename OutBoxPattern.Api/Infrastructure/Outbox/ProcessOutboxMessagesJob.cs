using System.Data;
using System.Text.Json;
using Dapper;
using MediatR;
using Microsoft.Extensions.Options;
using OutBoxPattern.Api.Application.Abstractions;
using Quartz;

namespace OutBoxPattern.Api.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessagesJob : IJob
{
  private readonly OutboxEventDispatcher _dispatcher;
  // private static readonly JsonSerializerSettings JsonSerializerSettings = new()
  // {
  //   TypeNameHandling = TypeNameHandling.All
  // };

  private readonly ILogger<ProcessOutboxMessagesJob> _logger;
  private readonly OutboxOptions _outboxOptions;
  private readonly IPublisher _publisher;

  private readonly ISqlConnectionFactory _sqlConnectionFactory;

  public ProcessOutboxMessagesJob(
    ISqlConnectionFactory sqlConnectionFactory,
    IPublisher publisher,
    IOptions<OutboxOptions> outboxOptions,
    ILogger<ProcessOutboxMessagesJob> logger,
    OutboxEventDispatcher dispatcher)
  {
    _sqlConnectionFactory = sqlConnectionFactory;
    _publisher = publisher;
    _logger = logger;
    _dispatcher = dispatcher;
    _outboxOptions = outboxOptions.Value;
  }

  public async Task Execute(IJobExecutionContext context)
  {
    _logger.LogInformation("Beginning to process outbox messages");

    using var connection = _sqlConnectionFactory.CreateConnection();
    using var transaction = connection.BeginTransaction();

    var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

    foreach (var outboxMessage in outboxMessages)
    {
      Exception? exception = null;

      try
      {
        var type = Type.GetType(outboxMessage.EventType);
        if (type == null) throw new InvalidOperationException($"Unknown type: {outboxMessage.EventType}");

        var domainEvent = JsonSerializer.Deserialize(
          outboxMessage.Payload, type)!;

        _logger.LogInformation("Processing event type {@EventType}", domainEvent);
        _dispatcher.Dispatch(domainEvent);
      }
      catch (Exception caughtException)
      {
        _logger.LogError(
          caughtException,
          "Exception while processing outbox message {MessageId}",
          outboxMessage.Id);

        exception = caughtException;
      }

      await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
    }

    transaction.Commit();

    _logger.LogInformation("Completed processing {MessageCount} outbox messages", outboxMessages.Count);
  }

  private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
    IDbConnection connection,
    IDbTransaction transaction)
  {
    var sql = $"""
               SELECT id AS Id, payload AS Payload, event_type AS EventType
               FROM outbox_messages
               WHERE processed_at IS NULL
               ORDER BY created_at
               LIMIT {_outboxOptions.BatchSize}
               FOR UPDATE
               """;

    var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
      sql,
      transaction: transaction);

    return outboxMessages.ToList();
  }

  private async Task UpdateOutboxMessageAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    OutboxMessageResponse outboxMessage,
    Exception? exception)
  {
    const string sql = """
                       
                                   UPDATE outbox_messages
                                   SET processed_at = @ProcessedOnUtc,
                                       error = @Error
                                   WHERE id = @Id
                       """;

    await connection.ExecuteAsync(
      sql,
      new
      {
        outboxMessage.Id,
        ProcessedOnUtc = TimeProvider.System.GetLocalNow().ToUniversalTime(),
        Error = exception?.ToString()
      },
      transaction);
  }

  internal sealed record OutboxMessageResponse(Guid Id, string Payload, string EventType);
}