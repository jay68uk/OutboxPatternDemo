using OutBoxPattern.Api.Messaging;

namespace OutBoxPattern.Api.Infrastructure.Outbox.EventHandlers;

public class UserCreatedEventHandler : IOutboxEventHandler<UserCreatedDomainEvent>
{
  private readonly ILogger<UserCreatedEventHandler> _logger;

  public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
  {
    _logger = logger;
  }

  public void Handle(UserCreatedDomainEvent domainEvent)
  {
    _logger.LogInformation("Event handled for user created: {domainEventEmail}", domainEvent.Email);
  }
}