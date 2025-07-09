namespace TodoApi.Models.Task;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Priority { get; set; }
}