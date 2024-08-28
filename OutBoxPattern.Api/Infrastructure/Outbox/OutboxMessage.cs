using System.ComponentModel.DataAnnotations;

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

  public Guid? Id { get; } = Guid.NewGuid();

  [MaxLength(100)] public string? EventType { get; private set; }

  [MaxLength(1000)] public string? Payload { get; private set; }

  public DateTimeOffset CreatedAt { get; private set; }
  public DateTimeOffset? ProcessedAt { get; } = null;

  public string? Error { get; } = string.Empty;
}