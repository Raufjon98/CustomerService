using MessagePack;

namespace Customer.Contracts.User.Responses;

[MessagePackObject]
public record UserResponse
{
    [Key(0)]
    public string? Id { get; set; }
    [Key(1)]
    public string? UserName { get; set; }
    [Key(2)]
    public string? FirstName { get; set; }
    [Key(3)]
    public string? LastName { get; set; }
    [Key(4)]
    public int Age { get; set; }
};