using Microsoft.EntityFrameworkCore;
using ToDoList.Models.ToDoGrpcService.Models;

namespace ToDoList.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    }
}
