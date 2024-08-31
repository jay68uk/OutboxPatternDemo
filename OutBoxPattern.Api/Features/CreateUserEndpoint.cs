using System.Net;
using Ardalis.Result;
using FastEndpoints;
using MediatR;
using OutBoxPattern.Api.Application.User;
using OutBoxPattern.Api.Infrastructure.Data;

namespace OutBoxPattern.Api.Features;

public class CreateUserEndpoint : Endpoint<CreateUserRequest>
{
  private readonly IMediator _mediator;
  private readonly UserRepository _userRepository;

  public CreateUserEndpoint(IMediator mediator, UserRepository userRepository)
  {
    _mediator = mediator;
    _userRepository = userRepository;
  }

  public override void Configure()
  {
    Post("/users");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new CreateUserCommand(req.FirstName, req.LastName, req.Email), ct);

    if (result.IsSuccess)
    {
      await SendCreatedAtAsync("GetUserById",
        result.Value.Id,
        result.Value,
        cancellation: ct);
      return;
    }

    if (result.IsConflict())
      await SendAsync("Email address has already been used!",
        (int)HttpStatusCode.Conflict,
        ct);
  }
}