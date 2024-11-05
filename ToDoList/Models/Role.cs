namespace ToDoList.Models;

public class Role
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public List<MyUser> MyUsers { get; set; }

    public Role()
    {
        MyUsers = new List<MyUser>();
    }
}