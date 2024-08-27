namespace OutBoxPattern.Api.Infrastructure.Outbox;

public class OutboxMessage
{
  public OutboxMessage(Guid id, string eventType, string payload)
  {
    EventType = eventType;
    Payload = payload;
    CreatedAt = TimeProvider.System.GetLocalNow().ToUniversalTime();
  }

  private OutboxMessage()
  {
  }

  public Guid Id { get; }
  public string EventType { get; private set; }
  public string Payload { get; private set; }
  public DateTimeOffset CreatedAt { get; private set; }
  public DateTimeOffset? ProcessedAt { get; private set; }

  public string? Error { get; }

  public void MarkAsProcessed()
  {
    ProcessedAt = TimeProvider.System.GetLocalNow();
  }
}