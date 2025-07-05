using TodoApi.Models.User;

namespace TodoApi.Models.Team;

public class TeamAdmin
{
    public long TeamId { get; set; }
    public Team Team { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

}