using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using ToDoList.ViewModels;

namespace ToDoList.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<UserI> _userManager;
    private readonly SignInManager<UserI> _signInManager;

    public AccountController(UserManager<UserI> userManager, SignInManager<UserI> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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