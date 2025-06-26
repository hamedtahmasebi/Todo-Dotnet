using Microsoft.EntityFrameworkCore;
using TodoApi.Models.Todo;

namespace TodoApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<TodoItem> Todos { get; set; }
    }
}