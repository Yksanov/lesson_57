using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers;

public class UserController : Controller
{
    private readonly TaskStoreContext _context;
    private readonly UserManager<UserI> _userManager;
    private readonly EmailService _emailService;

    public UserController(TaskStoreContext context, UserManager<UserI> userManager, EmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<IActionResult> Profile(int? id)
    {
        UserI user = await _userManager.GetUserAsync(User);

        if (id != null && await _userManager.Users.AnyAsync(u => u.Id == id))
        {
            user = await _userManager.FindByIdAsync(id.ToString());
        }
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
    {
        UserI user = await _userManager.GetUserAsync(User);
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (result.Succeeded)
        {
            string subject = "Пароль успешно изменен";
            string message = "Ваш пароль успено изменен!";
            await _emailService.SendEmailAsync(user.Email, subject, message);

            return RedirectToAction("Profile");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("Profile", user);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(string email, int age)
    {
        UserI user = await _userManager.GetUserAsync(User);
        user.Email = email;
        user.Age = age;

        await _userManager.UpdateAsync(user);
        string subject = "Данные профиля обновлены";
        string message = $"Ваши профил был изменен:\nEmail: {user.Email}\nВозраст: {user.Age}";

        await _emailService.SendEmailAsync(user.Email, subject, message);

        return RedirectToAction("Profile");
    }
}