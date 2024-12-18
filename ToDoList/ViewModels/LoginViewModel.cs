using System.ComponentModel.DataAnnotations;

namespace ToDoList.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email or UserName is required")]
    public string EmailOrUserName { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    public bool RememerMe { get; set; }
    public string? ReturnUrl { get; set; }
}