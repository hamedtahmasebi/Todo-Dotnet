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
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            return await _context.Tasks.Select(t => new TaskItemDto
            {
                Title = t.Title,
                UserId = t.UserId,
                Content = t.Content,
                Deadline = t.Deadline,
                Id = t.Id,
                Priority = t.Priority,
                SubtaskIds = t.Subtasks.Select(st => st.Id).ToList()
            }).Where(t => t.UserId == userId).ToArrayAsync();
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

        // PUT: api/Task/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

    }
}
