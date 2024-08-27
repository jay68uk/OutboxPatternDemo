using MediatR;
using OutBoxPattern.Api.Infrastructure.Data;

namespace OutBoxPattern.Api.Application.User;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Domain.User>
{
  private readonly UserRepository _userRepository;

  public CreateUserCommandHandler(UserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Domain.User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
  {
    var newUser =
      await _userRepository.CreateUserAsync(Guid.NewGuid(), request.FirstName, request.LastName, request.Email);

    return newUser;
  }
}