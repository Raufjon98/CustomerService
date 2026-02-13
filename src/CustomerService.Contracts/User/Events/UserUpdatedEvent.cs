namespace CustomerService.Contracts.User.Events;

public record UserUpdatedEvent
{
    public required string Id { get; init; }
    public DateTime UpdatedOnUtc { get; init; }
}