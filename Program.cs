using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
// Importujemy Twój folder z makiet¹
using Techniki_Internetowe_Proj.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// KRYTYCZNE: To musi tu byæ, ¿eby dzia³a³y kontrolery i widoki HTML
builder.Services.AddControllersWithViews();

// --- SEKCJA £UKASZA: Konfiguracja bezpieczeñstwa kont (ASP.NET Core Identity) ---
// UWAGA DLA MATEUSZA: Kiedy zrobisz klasê bazy danych (DbContext), 
// musisz tu dopisaæ .AddEntityFrameworkStores<NazwaTwojegoKontektstu>() na koñcu tej konfiguracji!
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // 1. Twarda polityka hase³
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // 2. Ochrona przed atakami (Brute-Force)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
})
// Wpinamy nasz¹ makietê bezpoœrednio w system Identity
.AddUserStore<TemporaryIdentityStore>()
.AddRoleStore<TemporaryIdentityStore>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// KRYTYCZNE: Musi byæ Authentication (Kto to jest?) przed Authorization (Co mu wolno?)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();