using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Models;

public class TaskStoreContext : IdentityDbContext<UserI, IdentityRole<int>, int>
{
    public DbSet<MyTask> MyTasks { get; set; }
    public TaskStoreContext(DbContextOptions<TaskStoreContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Brand>().HasMany(b => b.Products).WithOne(p => p.Brand).HasForeignKey();  List<Product> Products {get; set;}
        modelBuilder.Entity<MyTask>()
            .HasOne(t => t.UserCreator)
            .WithMany(u => u.CreatorTasks)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<MyTask>()
            .HasOne(t => t.UserExecutor)
            .WithMany(u => u.ExecutorTasks)
            .HasForeignKey(t => t.ExecutorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        base.OnModelCreating(modelBuilder);
    }
}