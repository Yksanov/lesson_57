using Microsoft.AspNetCore.Authorization;
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
            UserI user = await _userManager.FindByEmailAsync(model.EmailOrUserName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUserName);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден!");
                return View(model);
            }
            
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememerMe, false);
            if (signInResult.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "MyTask");
            }

            ViewBag.ReturnUrl = HttpContext.Request.Query["ReturnUrl"];
            ModelState.AddModelError(string.Empty, "Неправильное логин или пароль");
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
                UserName = model.UserName,
                Age = model.Age
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new {userId = user.Id, code = code},
                    protocol: HttpContext.Request.Scheme);
                _emailService.SendEmailAsync(model.Email, "Confirm your account",
                    $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>Перейти</a>");
                //return View("Для завершения регистрация проверьте электронную почту и перейдите по ссылке, указанной в письме");
                
                //------------------------------------------------------
                await _signInManager.SignInAsync(user, false);
                await _userManager.AddToRoleAsync(user, "user");

                string subject = "Добро пожаловать!";
                string message = $"Здравствуйте, {user.UserName}\n" +
                                 $"Ваш логин успешно зарегистрирован.\n" +
                                 $"Логин: {user.Email}\n" +
                                 $"Ссылка на профиль: <a href=\"http://localhost/User/Profile/{user.Id}\">Профиль</a>";
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
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
            return RedirectToAction("Index", "MyTask");
        else
            return View("Error");
    }
    
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}