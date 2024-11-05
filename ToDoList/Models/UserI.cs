using Microsoft.AspNetCore.Identity;

namespace ToDoList.Models;

public class UserI : IdentityUser<int>
{
    public int Age { get; set; }
}