using Ardalis.Result;
using MediatR;

namespace OutBoxPattern.Api.Application.User;

public sealed record CreateUserCommand(string FirstName, string LastName, string Email) : IRequest<Result<Domain.User>>;