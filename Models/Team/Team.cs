using Microsoft.EntityFrameworkCore;
using TodoApi.Models.User;

namespace TodoApi.Models.Team;

[Index(nameof(Name))]
public class Team
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ApplicationUser? Owner { get; set; }
    public string? OwnerId { get; set; }
    public ICollection<TeamAdmin> Admins { get; set; } = [];
    public ICollection<TeamMember> Members { get; set; } = [];

}