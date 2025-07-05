using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models.Team;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/Team
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<GetTeamDto[]>> GetTeams()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var teams = await _context.Teams
                .Where(t => t.Members.Any(m => m.UserId == userId))
                .Select(t => new GetTeamDto
                {
                    OwnerId = t.OwnerId,
                    AdminIds = t.Admins.Select(a => a.UserId).ToArray(),
                    MemberIds = t.Members.Select(m => m.UserId).ToArray(),
                    Name = t.Name
                }).ToArrayAsync();

            return teams;
        }

        // GET: api/Team/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetTeamDto>> GetTeam(long id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var team = await _context.Teams
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .Select(t => new GetTeamDto
            {
                OwnerId = t.OwnerId,
                AdminIds = t.Admins.Select(a => a.UserId).ToArray(),
                MemberIds = t.Members.Select(m => m.UserId).ToArray(),
                Name = t.Name
            }).FirstAsync();

            if (team == null) return NotFound();

            return team;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PatchTeam(long id, UpdateTeamDto updatedTeam)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var team = await _context.Teams.FindAsync(id);

            if (team == null) return NotFound();
            if (userId == null || !team.Admins.Any(a => a.UserId == userId)) return Unauthorized();

            if (!string.IsNullOrEmpty(updatedTeam.Name))
                team.Name = updatedTeam.Name;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Team
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<GetTeamDto>> PostTeam(CreateTeamDto data)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var existingTeam = await _context.Teams.AnyAsync(t => t.Name == data.Name);
            if (existingTeam) return Conflict(new ProblemDetails
            {
                Title = "Duplicate Team",
                Detail = "You already have a team with the same name",

            });

            Team newTeam = new() { Name = data.Name, Admins = [], Members = [] };

            TeamAdmin teamAdmin = new()
            { Team = newTeam, UserId = userId };
            TeamMember teamMember = new()
            { Team = newTeam, UserId = userId };

            newTeam.OwnerId = userId;
            newTeam.Admins.Add(teamAdmin);
            newTeam.Members.Add(teamMember);

            _context.Teams.Add(newTeam);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = newTeam.Id }, new GetTeamDto
            {
                OwnerId = newTeam.OwnerId,
                AdminIds = newTeam.Admins.Select(a => a.UserId).ToArray(),
                MemberIds = newTeam.Members.Select(m => m.UserId).ToArray(),
                Name = newTeam.Name
            });
        }

        // DELETE: api/Team/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(long id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (
                userId == null ||
                !team.Admins.Any(a => a.UserId == userId)
            ) return Unauthorized();


            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
