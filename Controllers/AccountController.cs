using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Techniki_Internetowe_Proj.ViewModels;

namespace Techniki_Internetowe_Proj.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // Wstrzykiwanie zależności (Dependency Injection)
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // --- REJESTRACJA ---

        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Zwraca widok formularza rejestracji (HTML)
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tworzymy nowego użytkownika
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

                // UserManager automatycznie haszuje hasło i zapisuje w bazie
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Automatyczne logowanie po udanej rejestracji
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // Jeśli coś poszło nie tak (np. hasło za słabe, email zajęty), wyłapujemy błędy
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // --- LOGOWANIE ---

        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Zwraca widok formularza logowania (HTML)
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Weryfikacja danych logowania
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Błąd logowania (celowo ogólny, żeby nie ułatwiać życia hakerom)
                ModelState.AddModelError(string.Empty, "Nieudana próba logowania. Sprawdź e-mail i hasło.");
            }
            return View(model);
        }

        // --- WYLOGOWANIE ---

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}