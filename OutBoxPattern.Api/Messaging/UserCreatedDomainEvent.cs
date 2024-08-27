using OutBoxPattern.Api.Domain;

namespace OutBoxPattern.Api.Messaging;

public class UserCreatedDomainEvent(User user) : IDomainEvent
{
  public Guid UserId { get; } = user.Id;

  public string LastName { get; } = user.LastName;

  public string FirstName { get; } = user.FirstName;

  public string Email { get; } = user.Email;
}