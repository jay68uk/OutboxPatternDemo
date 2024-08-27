using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OutBoxPattern.Api.Domain;
using OutBoxPattern.Api.Infrastructure.Outbox;

namespace OutBoxPattern.Api.Infrastructure.Data;

public class UserRepository
{
  private readonly UserDbContext _context;

  public UserRepository(UserDbContext context)
  {
    _context = context;
  }

  public async Task<User> CreateUserAsync(Guid id, string firstname, string lastname, string email)
  {
    var user = User.Create(firstname, lastname, email);
    _context.Users.Add(user);

    var outboxMessage = new OutboxMessage(
      Guid.NewGuid(),
      "UserCreated",
      JsonSerializer.Serialize(user)
    );
    _context.OutboxMessages.Add(outboxMessage);

    await _context.SaveChangesAsync();
    return user;
  }

  private void GenerateOutboxMessage(OutboxMessage outboxMessage)
  {
    //var tracker = ChangeTracker.Entries();
  }

  public async Task<List<OutboxMessage>> GetUnprocessedOutboxMessagesAsync()
  {
    return await _context.OutboxMessages.Where(x => x.ProcessedAt == null).ToListAsync();
  }

  public async Task MarkAsProcessedAsync(OutboxMessage message)
  {
    await _context.SaveChangesAsync();
  }

  public async Task UpdateUserEmailSentDateAsync(Guid userId)
  {
    var user = await _context.Users.FindAsync(userId);
    if (user != null)
    {
      user.UpdateEmailSentAtAsync(TimeProvider.System.GetLocalNow());
      await _context.SaveChangesAsync();
    }
  }
}