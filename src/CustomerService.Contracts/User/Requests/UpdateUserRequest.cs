using MessagePack;

namespace Customer.Contracts.User.Requests;

[MessagePackObject]
public record UpdateUserRequest
{
    [Key(0)]
    public string? Username { get; set; }
    [Key(1)]
    public string? FirstName { get; set; }
    [Key(2)]
    public string? LastName { get; set; }
    [Key(3)]
    public string? MiddleName { get; set; }
    [Key(4)]
    public string? Email { get; set; }
    [Key(5)]
    public string? Phone { get; set; } 
};