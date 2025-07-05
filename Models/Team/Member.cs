using TodoApi.Models.User;

namespace TodoApi.Models.Team;


public class TeamMember
{
    public long TeamId { get; set; }
    public required Team Team { get; set; }
    public ApplicationUser? User { get; set; }
    public required string UserId { get; set; }

}