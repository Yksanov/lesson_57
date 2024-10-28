using ToDoList.Services;

namespace ToDoList.Models;

public class MyTask
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public DateOnly CreatedDate { get; set; }
    public DateOnly OpenDate { get; set; }
    public DateOnly CloseDate { get; set; }
    public string UserName { get; set; }
}