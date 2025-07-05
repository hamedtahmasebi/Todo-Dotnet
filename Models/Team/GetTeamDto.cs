using System.Collections;

namespace TodoApi.Models.Team;

public class GetTeamDto
{
    public required string Name { get; set; }
    public required string OwnerId { get; set; }
    public required string[] AdminIds { get; set; }
    public required string[] MemberIds { get; set; } = [];
}