namespace OutBoxPattern.Api.Infrastructure.Outbox;

public class OutboxEventDispatcher
{
  private readonly IServiceProvider _serviceProvider;

  public OutboxEventDispatcher(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public void Dispatch(object domainEvent)
  {
    var eventType = domainEvent.GetType();
    var handlerType = typeof(IOutboxEventHandler<>).MakeGenericType(eventType);
    var handler = _serviceProvider.GetService(handlerType);

    if (handler == null)
      throw new InvalidOperationException($"No handler registered for domain event type: {eventType.Name}");

    var method = handlerType.GetMethod("Handle");
    method.Invoke(handler, new[] { domainEvent });
  }
}