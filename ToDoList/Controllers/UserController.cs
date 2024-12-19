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
            user = await _context.Users.Include(u => u.CreatorTasks).FirstOrDefaultAsync(u => u.Id == id);
        }
        else
        {
            user = await _context.Users.Include(u => u.CreatorTasks).FirstOrDefaultAsync(u => u.Id == user.Id);
        }
        if (user == null)
        {
            return NotFound();
        }

        int createCount = user.CreatorTasks?.Count ?? 0;
        ViewBag.CreatTaskCount = createCount;
        
        int executorCount = user.ExecutorTasks?.Count ?? 0;
        ViewBag.ExecutorTaskCount = executorCount;

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
    public async Task<IActionResult> EditProfile(string userName,string email, int age)
    {
        UserI user = await _userManager.GetUserAsync(User);

        if (user == null)
            return NotFound();
        
        user.UserName = userName;
        user.Email = email;
        user.Age = age;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            string subject = "Данные профиля обновлены";
            string message = $"Ваши профил был изменен:\nИмя пользователя: {user.UserName}\nEmail: {user.Email}\nВозраст: {user.Age}";

            await _emailService.SendEmailAsync(user.Email, subject, message);
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return RedirectToAction("Profile");
    }

    [HttpPost]
    public async Task<IActionResult> UserData(int userId)
    {
        UserI user = await _context.Users.Include(u => u.CreatorTasks).Include(u => u.ExecutorTasks).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound();
        }

        int taskCount = user.CreatorTasks?.Count ?? 0;
        int executorCount = user.ExecutorTasks?.Count ?? 0;
        string subject = "Ваши данные";
        string message =  $"Публичная информация по вашему профилю:\n" +
                          $"Имя пользователя: {user.UserName}\n" +
                          $"Почта: {user.Email}\n" +
                          $"Возраст: {user.Age}\n" +
                          $"Количество задач: {taskCount}\n" + 
                          $"Количество взятых задач: {executorCount}";
        await _emailService.SendEmailAsync(user.Email, subject, message);

        TempData["Message"] = "Ваши данные отправлены на почту";
        return RedirectToAction("Profile", new { id = userId });
    }
}