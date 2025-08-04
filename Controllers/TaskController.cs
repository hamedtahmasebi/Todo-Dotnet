using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models.Task;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/Task
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks([FromQuery] TaskFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var validSortBy = new[] { "Title", "Content", "Deadline", "Priority" };
            var validSortDirection = new[] { "asc", "desc" };


            if (filter.SortBy != null && filter.SortDirection != null && filter.SortBy.Length == filter.SortDirection.Length)
            {
                for (int i = 0; i < filter.SortBy.Length; i++)
                {
                    if (!validSortBy.Contains(filter.SortBy[i]) || !validSortDirection.Contains(filter.SortDirection[i]))
                    {
                        return BadRequest("Invalid sorting parameters.");
                    }
                }
            }


            var query = _context.Tasks.Select(t => new TaskItemDto
            {
                Title = t.Title,
                UserId = t.UserId,
                Content = t.Content,
                Deadline = t.Deadline,
                Id = t.Id,
                Priority = t.Priority,
                SubtaskIds = t.Subtasks.Select(st => st.Id).ToList()
            })
            .Where(t => t.UserId == userId &&
                        (string.IsNullOrEmpty(filter.SearchString) || t.Title.Contains(filter.SearchString, StringComparison.OrdinalIgnoreCase)));


            if (filter.SortBy != null && filter.SortDirection != null)
            {
                for (int i = 0; i < filter.SortBy.Length; i++)
                {
                    if (filter.SortDirection[i].Equals("asc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy(t => EF.Property<object>(t, filter.SortBy[i]));
                    }
                    else if (filter.SortDirection[i].Equals("desc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderByDescending(t => EF.Property<object>(t, filter.SortBy[i]));
                    }
                }
            }



            return await query
                .Skip((filter.Page - 1) * filter.Size)
                .Take(filter.Size)
                .ToListAsync();
        }

        // GET: api/Task/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TaskItemDto>> GetTaskItem(long id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();


            var taskItem = await _context.Tasks.Select(t => new TaskItemDto
            {
                Title = t.Title,
                UserId = t.UserId,
                Content = t.Content,
                Deadline = t.Deadline,
                Id = t.Id,
                Priority = t.Priority,
                SubtaskIds = t.Subtasks.Select(st => st.Id).ToList()
            }).Where(t => t.UserId == userId).FirstAsync();

            if (taskItem == null)
            {
                return NotFound();
            }

            return taskItem;
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> PatchTaskItem(long id, [FromBody] UpdateTaskDto data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var task = await _context.Tasks.FindAsync(id);

            if (task == null) return NotFound();
            if (task.UserId != userId) return Unauthorized();

            if (data.Title != null) task.Title = data.Title;
            if (data.Content != null) task.Content = data.Content;
            if (data.Priority != null) task.Priority = data.Priority;
            if (data.Deadline != null) task.Deadline = data.Deadline;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Task
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TaskItemDto>> PostTaskItem(CreateTaskDto data)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var task = new TaskItem
            {
                Title = data.Title,
                Content = data.Content,
                Deadline = data.Deadline,
                Priority = data.Priority,
                UserId = userId
            };


            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTaskItem", new { id = task.Id }, task);
        }

        // DELETE: api/Task/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(long id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var taskItem = await _context.Tasks
                .Select(t => t)
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstAsync();

            if (taskItem == null) return NotFound();

            _context.Tasks.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        public class TaskFilter
        {
            public string? SearchString { get; set; }
            public string[]? SortBy { get; set; }
            public string[]? SortDirection { get; set; }
            public int Page { get; set; } = 1;
            public int Size { get; set; } = 10;
        }


    }
}
