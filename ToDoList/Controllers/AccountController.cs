using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<UserI> _userManager;
    private readonly SignInManager<UserI> _signInManager;
    private readonly EmailService _emailService;

    public AccountController(UserManager<UserI> userManager, SignInManager<UserI> signInManager, EmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ViewBag.ReturnUrl = HttpContext.Request.Query["ReturnUrl"];
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            UserI user = await _userManager.FindByEmailAsync(model.Email);
            Microsoft.AspNetCore.Identity.SignInResult signInResult =
                await _signInManager.PasswordSignInAsync(user, model.Password, model.RememerMe, false);
            if (signInResult.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "MyTask");
            }

            ViewBag.ReturnUrl = HttpContext.Request.Query["ReturnUrl"];
            ModelState.AddModelError(string.Empty, "Error");
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "MyTask");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            UserI user = new UserI()
            {
                Email = model.Email,
                UserName = model.Email,
                Age = model.Age
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                await _userManager.AddToRoleAsync(user, "user");

                string subject = "Добро пожаловать!";
                string message = $"Здравствуйте, {user.UserName}\n" +
                                 $"Ваш логин успешно зарегистрирован.\n" +
                                 $"Логин: {user.Email}";
                await _emailService.SendEmailAsync(user.Email, subject, message);
                
                return RedirectToAction("Index", "MyTask");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}