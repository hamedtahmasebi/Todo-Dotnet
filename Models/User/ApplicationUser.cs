using Microsoft.AspNetCore.Identity;
using TodoApi.Models.Team;

namespace TodoApi.Models.User;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }


    public ICollection<TeamAdmin> AdminOf { get; set; } = [];
    public ICollection<TeamMember> MemberOf { get; set; } = [];
}