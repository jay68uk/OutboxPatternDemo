using OutBoxPattern.Api.Domain;

namespace OutBoxPattern.Api.Messaging;

public class UserCreatedDomainEvent : IDomainEvent
{
  public UserCreatedDomainEvent()
  {
  }

  public UserCreatedDomainEvent(User user)
  {
    UserId = user.Id;
    LastName = user.LastName;
    FirstName = user.FirstName;
    Email = user.Email;
  }

  public Guid UserId { get; set; }
  public string LastName { get; set; }
  public string FirstName { get; set; }
  public string Email { get; set; }
}