
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CustomerService.Api.Domain;

public class User : IdentityUser
{
    public string? FirstName { get; set; } = ""; 
    public string? LastName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public DateOnly DateOfBirth { get; set; }
    [NotMapped]
    public string? Role { get; set; }
    [NotMapped]
    public string? RoleId { get; set; }
    [NotMapped]
    public IEnumerable<SelectListItem>? RoleList { get; set; }

    public bool IsDelete { get; set; } = false;
}