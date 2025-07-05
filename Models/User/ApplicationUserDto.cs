using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace TodoApi.Models.User;

public class ApplicationUserDto
{
    public required string Id { get; set; }
    public required string Username { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}