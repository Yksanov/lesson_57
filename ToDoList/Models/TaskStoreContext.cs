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
        
        // modelBuilder.Entity<Role>().HasData(new Role {Id = 1, Name = "admin"});
        // modelBuilder.Entity<Role>().HasData(new Role {Id = 2, Name = "user"});
        // modelBuilder.Entity<MyUser>().HasData(new MyUser {Id = 2, Email = "admin@admin.com", Password = "1qwe@QWE", UserName = "admin", RoleId = 1});
        
        base.OnModelCreating(modelBuilder);
    }
}