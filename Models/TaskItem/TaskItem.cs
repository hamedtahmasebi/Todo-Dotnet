using System.Collections.ObjectModel;
using TodoApi.Models.User;


namespace TodoApi.Models.Task;

public enum ETaskPriority : int
{
    VeryLow = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    VeryHigh = 4,
    Critical = 5
}

public class TaskItem
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
    public DateTime? Deadline { get; set; }
    public ETaskPriority? Priority { get; set; } = null;

    public Collection<TaskItem> Subtasks { get; set; } = [];

    public ApplicationUser User { get; set; }
    public string UserId { get; set; }

}