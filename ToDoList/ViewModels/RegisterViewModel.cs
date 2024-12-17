using System.ComponentModel.DataAnnotations;

namespace ToDoList.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Age is required")]
    public int Age { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; }
}