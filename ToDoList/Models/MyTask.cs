using Microsoft.Build.Framework;
using ToDoList.Services;

namespace ToDoList.Models;

public class MyTask
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public Priority Priority { get; set; }
    public Status Status { get; set; }
    public DateOnly CreatedDate { get; set; }
    public DateOnly OpenDate { get; set; }
    public DateOnly CloseDate { get; set; }
    [Required]
    public string? UserName { get; set; }
}