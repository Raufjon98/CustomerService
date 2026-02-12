using MessagePack;

namespace Customer.Contracts.User.Requests;

[MessagePackObject]
public record UpdateUserPasswordRequest
{
    [Key(0)]
    public required string OldPassword { get; set; }
    [Key(1)]
    public required string NewPassword { get; set; }
};