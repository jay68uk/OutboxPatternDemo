using OutBoxPattern.Api.Messaging;

namespace OutBoxPattern.Api.Infrastructure.Outbox;

public interface IOutboxEventHandler<in TEvent> where TEvent : IDomainEvent
{
  void Handle(TEvent domainEvent);
}