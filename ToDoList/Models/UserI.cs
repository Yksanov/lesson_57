using Microsoft.AspNetCore.Identity;

namespace ToDoList.Models;

public class UserI : IdentityUser<int>
{
    public int Age { get; set; }

    public List<MyTask> CreatorTasks { get; set; }
    public List<MyTask> ExecutorTasks { get; set; }
}