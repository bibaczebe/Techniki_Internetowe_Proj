using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Techniki_Internetowe_Proj.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC: kontrolery + widoki
builder.Services.AddControllersWithViews();

// ===== ASP.NET Core Identity: uwierzytelnianie i autoryzacja kont =====
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Polityka hasel
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Blokada konta po nieudanych probach logowania (ochrona przed brute-force)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Email musi byc unikalny
    options.User.RequireUniqueEmail = true;
})
    // Tymczasowy magazyn trzymajacy dane w pamieci (bez bazy).
    // PO DODANIU BAZY (EF Core) podmieniacie te dwie linie na jedna:
    //     .AddEntityFrameworkStores<NazwaWaszegoKontekstu>()
    .AddUserStore<TemporaryUserStore>()
    .AddRoleStore<TemporaryRoleStore>()
    .AddDefaultTokenProviders();

// Sciezki uzywane przez Identity (zgodne z AccountController)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kolejnosc jest wazna: najpierw Authentication (kto to jest), potem Authorization (co mu wolno)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ===== Seed: rola Admin + jedno konto admina (zeby pokazac autoryzacje na obronie) =====
// Ten blok mozna usunac po podlaczeniu bazy i utworzeniu wlasnego konta admina.
await SeedAdminAsync(app);

app.Run();

static async Task SeedAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = "Admin";
    const string adminEmail = "admin@smacza.pl";
    const string adminPassword = "Admin123!";

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, adminRole);
        }
    }
}
