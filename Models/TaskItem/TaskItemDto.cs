namespace TodoApi.Models.Task;


public class TaskItemDto
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Priority { get; set; }

    public List<long> SubtaskIds { get; set; } = [];

    public required string UserId { get; set; }

}