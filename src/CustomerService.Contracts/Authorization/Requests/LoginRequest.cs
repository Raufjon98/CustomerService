using MessagePack;
namespace CustomerService.Contracts.Authorization.Requests;

[MessagePackObject]
public record LoginRequest
{
    [Key(0)]
    public required string Username { get; set; }
    [Key(1)]
    public required string Password { get; set; }
};