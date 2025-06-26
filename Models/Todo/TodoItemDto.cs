namespace TodoApi.Models.Todo;


public class TodoItemDto
{
    public long Id { get; set; }
    public string? Title { get; set; }

    public string? Desc { get; set; }

    public bool IsCompleted { get; set; }
}


