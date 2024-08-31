using Ardalis.Result;
using OutBoxPattern.Api.Application.Exceptions;
using OutBoxPattern.Api.Domain;

namespace OutBoxPattern.Api.Infrastructure.Data;

public class UserRepository
{
  private readonly UserDbContext _context;

  public UserRepository(UserDbContext context)
  {
    _context = context;
  }

  public async Task<Result<User>> CreateUserAsync(Guid id, string firstname, string lastname, string email)
  {
    try
    {
      var user = User.Create(firstname, lastname, email);
      _context.Users.Add(user);

      await _context.SaveChangesAsync();
      return Result.Success(user);
    }
    catch (ConcurrencyException e)
    {
      return Result<User>.Conflict();
    }
  }

  public async Task UpdateUserEmailSentDateAsync(Guid userId)
  {
    var user = await _context.Users.FindAsync(userId);
    if (user != null)
    {
      user.UpdateEmailSentAtAsync(TimeProvider.System.GetLocalNow().ToUniversalTime());
      await _context.SaveChangesAsync();
    }
  }
}