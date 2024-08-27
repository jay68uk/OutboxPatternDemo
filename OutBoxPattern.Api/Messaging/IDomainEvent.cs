using MediatR;

namespace OutBoxPattern.Api.Messaging;

public interface IDomainEvent : INotification
{
}