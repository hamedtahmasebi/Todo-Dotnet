using TodoApi.Models.User;
using TodoApi.Models.Team;


namespace TodoApi.Services.Team;

public interface ITeamService
{
    Task<GetTeamDto[]> GetTeamsForUserAsync(string userId);
    Task<GetTeamDto?> GetTeamByIdAsync(long teamId, string userId);
    Task<GetTeamDto?> CreateTeamAsync(string userId, CreateTeamDto dto);
    Task<bool> UpdateTeamAsync(long teamId, string userId, UpdateTeamDto dto);
    Task<bool> DeleteTeamAsync(long teamId, string userId);
}