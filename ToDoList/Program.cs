using System.IO.Compression;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using ToDoList.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.CacheProfiles.Add("EnableCaching", new CacheProfile()
    {
        Duration = 60,
        Location = ResponseCacheLocation.Any,
        NoStore = false
    });
    options.CacheProfiles.Add("NoCaching", new CacheProfile()
    {
        NoStore = true
    });
} );

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add(new DeflateCompressionProvider());
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
//--------------------------------------------------------

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TaskStoreContext>(options => options.UseNpgsql(connection))
    .AddIdentity<UserI, IdentityRole<int>>(
        options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
        }).AddEntityFrameworkStores<TaskStoreContext>();
//--------------------------------------------------------
builder.Services.AddMemoryCache();
//--------------------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

var app = builder.Build();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var userManager = services.GetRequiredService<UserManager<UserI>>();
    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    
    await AdminInirializer.SeedRolesAndAdmin(rolesManager, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding the database.");
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//--------------------------------------------------------
app.UseResponseCaching();
app.UseResponseCompression();
//--------------------------------------------------------
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MyTask}/{action=Index}/{id?}");

app.Run();