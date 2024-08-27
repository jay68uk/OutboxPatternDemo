using OutBoxPattern.Api.Domain.Abstractions;
using OutBoxPattern.Api.Messaging;

namespace OutBoxPattern.Api.Domain;

public class User : Entity
{
  private User(Guid id, string firstName, string lastname, string email) : base(id)
  {
    FirstName = firstName;
    LastName = lastname;
    Email = email;
  }

  private User()
  {
  }

  public string FirstName { get; private set; }
  public string LastName { get; private set; }
  public string Email { get; private set; }
  public DateTimeOffset? EmailSentAt { get; private set; }

  public static User Create(string firstName, string lastName, string email)
  {
    var user = new User(Guid.NewGuid(), firstName, lastName, email);

    user.RaiseDomainEvent(new UserCreatedDomainEvent(user));

    return user;
  }

  public void UpdateEmailSentAtAsync(DateTimeOffset sentDate)
  {
    EmailSentAt = sentDate.ToUniversalTime();
  }
}