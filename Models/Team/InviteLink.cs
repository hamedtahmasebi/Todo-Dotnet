namespace TodoApi.Models.Team;

public enum ETeamInviteLinkType
{
    Anyone,
    ByEmail

}

public class TeamInviteLink
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required Team Team { get; set; }
    public long TeamId { get; set; }

    public ETeamInviteLinkType Type { get; set; } = ETeamInviteLinkType.Anyone;

    public string? InvitedEmail { get; set; }
}