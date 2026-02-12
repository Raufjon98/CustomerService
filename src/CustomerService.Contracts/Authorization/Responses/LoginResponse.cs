using MessagePack;

namespace CustomerService.Contracts.Authorization.Responses;

[MessagePackObject]
public record LoginResponse
{
    [Key(0)]
    public string? Token { get; set; }
};