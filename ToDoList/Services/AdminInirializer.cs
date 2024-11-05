using Microsoft.AspNetCore.Identity;
using ToDoList.Models;

namespace ToDoList.Services;

public class AdminInirializer
{
    public static async Task SeedRolesAndAdmin(RoleManager<IdentityRole<int>> roleManager, UserManager<UserI> _userManager)
    {
        string adminEmail = "admin@admin.com";
        string adminPassword = "1qwe@QWE";

        var roles = new[] { "admin", "user" };
        
        foreach (var role in roles)
        {
            if (await roleManager.FindByNameAsync(role) is null)
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        if (await _userManager.FindByNameAsync(adminEmail) == null)
        {
            UserI admin = new UserI { Email = adminEmail, UserName = adminEmail };
            IdentityResult result = await _userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(admin, "admin");
        }
    }
}