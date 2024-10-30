using ToDoList.Models;

namespace ToDoList.ViewModels;

public class TaskModels
{
    public IEnumerable<MyTask> Tasks { get; set; }
    public PageViewModel PageViewModel { get; set; }
}