using MessagePack;

namespace CustomerService.Contracts.Authorization.Requests;

[MessagePackObject]
public record RegisterRequest
{
    [Key(0)]
    public required string Username { get; set; }
    [Key(1)]
    public required string Firstname { get; set; }
    [Key(2)]
    public required string Lastname { get; set; }
    [Key(3)]
    public required string Email { get; set; }
    [Key(4)]
    public required string Password { get; set; }
    [Key(5)]
    public DateOnly DateOfBirth { get; set; }
};