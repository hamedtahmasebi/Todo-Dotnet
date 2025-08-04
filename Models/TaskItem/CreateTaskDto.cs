namespace TodoApi.Models.Task;

public class CreateTaskDto
{
    public required string Title { get; set; }
    public string? Content { get; set; }
    public DateTime? Deadline { get; set; }
    public ETaskPriority? Priority { get; set; }

}