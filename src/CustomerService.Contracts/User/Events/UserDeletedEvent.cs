using System.Security;

namespace CustomerService.Contracts.User.Events;

public record UserDeletedEvent
{
    public required string Id { get; init; }
    public DateTime DeletedOnUtc { get; init; }
}