using Microsoft.EntityFrameworkCore;

namespace ToDoList.Models;

public class TaskStoreContext : DbContext
{
    public DbSet<MyTask> MyTasks { get; set; }
    public TaskStoreContext(DbContextOptions<TaskStoreContext> options) : base(options) { }
}