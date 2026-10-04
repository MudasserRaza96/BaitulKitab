using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Baitul_Kitab.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Baitul_Kitab.Models;
using Baitul_Kitab.BAL.Services;
using Baitul_Kitab.BAL.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // Validate security stamp on every request so role changes take effect immediately
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LogoutPath = "/Identity/Account/Logout";
    options.ReturnUrlParameter = "ReturnUrl";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(options =>
{
    // Login/Register/etc. must be reachable before authentication
    options.Conventions.AllowAnonymousToAreaFolder("Identity", "/Account");
});

// Standard authorization: endpoints with [Authorize] require authentication/roles
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICategories, CategoriesServices>();
builder.Services.AddScoped<ILanguages, LanguagesServices>();
builder.Services.AddScoped<IAuthors, AuthorsServices>();
builder.Services.AddScoped<IBooks, BooksServices>();
builder.Services.AddScoped<IUserBooks, UserBooksServices>();
builder.Services.AddScoped<ICartService, CartService>();

// Advertisement placement structure (configured via the "Ads" section of appsettings.json)
builder.Services.Configure<Baitul_Kitab.Ads.AdSettings>(builder.Configuration.GetSection(Baitul_Kitab.Ads.AdSettings.SectionName));
builder.Services.AddSingleton<Baitul_Kitab.Ads.IAdPlacementProvider, Baitul_Kitab.Ads.AdPlacementProvider>();

var app = builder.Build();

// Seed Admin role and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
