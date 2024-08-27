namespace OutBoxPattern.Api.Features;

public record CreateUserRequest(string FirstName, string LastName, string Email);