using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Techniki_Internetowe_Proj.Models;

namespace Techniki_Internetowe_Proj.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // ================================================================
    //  DANE TYMCZASOWE (NA SZTYWNO)
    //  UWAGA: poniższe listy to dane przykładowe trzymane w pamięci.
    //  Służą TYLKO do zbudowania i pokazania frontendu.
    //  Backend (EF Core / baza danych) PODMIENI je później na dane z bazy.
    // ================================================================

    // Twardo zdefiniowana lista jednostek miary (używana w formularzu dodawania).
    private static readonly List<string> Units = new List<string>
    {
        "szt.", "g", "dag", "kg", "ml", "l", "szklanka", "łyżka", "łyżeczka", "szczypta", "ząbek"
    };

    // Twardo zdefiniowana lista dostępnych składników (do formularza i do "lodówki").
    private static readonly List<string> AllIngredients = new List<string>
    {
        "bulion warzywny", "budyń waniliowy", "bułka tarta", "cebula", "cukier", "cynamon",
        "czosnek", "dżem", "jabłka", "jajka", "liść laurowy", "makaron", "makaron spaghetti",
        "masło", "mąka", "mięso mielone", "mięso na rosół", "mleko", "natka pietruszki", "olej",
        "oregano", "parmezan", "passata pomidorowa", "pieprz", "proszek do pieczenia", "ryż",
        "schab", "sól", "spód z ciastek", "szczypiorek", "śmietana", "twaróg", "włoszczyzna",
        "ziele angielskie", "ziemniaki"
    };

    // Lista kategorii
    private static readonly List<Category> Categories = new List<Category>
    {
        new Category { Id = 1, Name = "Śniadania" },
        new Category { Id = 2, Name = "Obiady" },
        new Category { Id = 3, Name = "Zupy" },
        new Category { Id = 4, Name = "Desery" }
    };

    // Lista przepisów
    private static readonly List<Recipe> Recipes = new List<Recipe>
    {
        new Recipe
        {
            Id = 1,
            Title = "Naleśniki z dżemem",
            ImageUrl = "/images/nalesniki.svg",
            PrepTimeMinutes = 25,
            CategoryId = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "jajka", Amount = 2, Unit = "szt." },
                new Ingredient { Name = "mąka", Amount = 1, Unit = "szklanka" },
                new Ingredient { Name = "mleko", Amount = 1, Unit = "szklanka" },
                new Ingredient { Name = "sól", Amount = 1, Unit = "szczypta" },
                new Ingredient { Name = "olej", Amount = 1, Unit = "łyżka" },
                new Ingredient { Name = "dżem", Amount = 3, Unit = "łyżka" }
            },
            Steps = new List<string>
            {
                "Wymieszaj jajka, mąkę, mleko i sól na gładkie ciasto.",
                "Rozgrzej patelnię z odrobiną oleju.",
                "Smaż cienkie naleśniki z obu stron na złoty kolor.",
                "Posmaruj dżemem i zwiń."
            }
        },
        new Recipe
        {
            Id = 2,
            Title = "Jajecznica na maśle",
            ImageUrl = "/images/jajecznica.svg",
            PrepTimeMinutes = 10,
            CategoryId = 1,
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "jajka", Amount = 3, Unit = "szt." },
                new Ingredient { Name = "masło", Amount = 1, Unit = "łyżka" },
                new Ingredient { Name = "sól", Amount = 1, Unit = "szczypta" },
                new Ingredient { Name = "pieprz", Amount = 1, Unit = "szczypta" },
                new Ingredient { Name = "szczypiorek", Amount = 1, Unit = "łyżka" }
            },
            Steps = new List<string>
            {
                "Rozpuść masło na patelni.",
                "Wbij jajka i mieszaj na małym ogniu.",
                "Dopraw solą i pieprzem, posyp szczypiorkiem."
            }
        },
        new Recipe
        {
            Id = 3,
            Title = "Spaghetti Bolognese",
            ImageUrl = "/images/spaghetti.svg",
            PrepTimeMinutes = 45,
            CategoryId = 2,
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "makaron spaghetti", Amount = 400, Unit = "g" },
                new Ingredient { Name = "mięso mielone", Amount = 400, Unit = "g" },
                new Ingredient { Name = "cebula", Amount = 1, Unit = "szt." },
                new Ingredient { Name = "czosnek", Amount = 2, Unit = "ząbek" },
                new Ingredient { Name = "passata pomidorowa", Amount = 500, Unit = "ml" },
                new Ingredient { Name = "oregano", Amount = 1, Unit = "łyżeczka" },
                new Ingredient { Name = "parmezan", Amount = 50, Unit = "g" }
            },
            Steps = new List<string>
            {
                "Podsmaż cebulę i czosnek.",
                "Dodaj mięso i smaż do zrumienienia.",
                "Wlej passatę, dopraw oregano i duś 20 minut.",
                "Ugotuj makaron al dente i podaj z sosem oraz parmezanem."
            }
        },
        new Recipe
        {
            Id = 4,
            Title = "Kotlet schabowy z ziemniakami",
            ImageUrl = "/images/schabowy.svg",
            PrepTimeMinutes = 60,
            CategoryId = 2,
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "schab", Amount = 4, Unit = "szt." },
                new Ingredient { Name = "jajka", Amount = 2, Unit = "szt." },
                new Ingredient { Name = "bułka tarta", Amount = 1, Unit = "szklanka" },
                new Ingredient { Name = "mąka", Amount = 3, Unit = "łyżka" },
                new Ingredient { Name = "sól", Amount = 1, Unit = "szczypta" },
                new Ingredient { Name = "pieprz", Amount = 1, Unit = "szczypta" },
                new Ingredient { Name = "ziemniaki", Amount = 1, Unit = "kg" },
                new Ingredient { Name = "olej", Amount = 100, Unit = "ml" }
            },
            Steps = new List<string>
            {
                "Rozbij mięso tłuczkiem i dopraw solą oraz pieprzem.",
                "Obtocz w mące, jajku i bułce tartej.",
                "Smaż na rozgrzanym oleju z obu stron.",
                "Podawaj z gotowanymi ziemniakami."
            }
        },
        new Recipe
        {
            Id = 5,
            Title = "Rosół z makaronem",
            ImageUrl = "/images/rosol.svg",
            PrepTimeMinutes = 90,
            CategoryId = 3,
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "mięso na rosół", Amount = 1, Unit = "kg" },
                new Ingredient { Name = "włoszczyzna", Amount = 1, Unit = "szt." },
                new Ingredient { Name = "liść laurowy", Amount = 2, Unit = "szt." },
                new Ingredient { Name = "ziele angielskie", Amount = 4, Unit = "szt." },
                new Ingredient { Name = "sól", Amount = 2, Unit = "łyżeczka" },
                new Ingredient { Name = "makaron", Amount = 200, Unit = "g" },
                new Ingredient { Name = "natka pietruszki", Amount = 1, Unit = "łyżka" }
            },
            Steps = new List<string>
            {
                "Zalej mięso zimną wodą i zagotuj, zbierz szumowiny.",
                "Dodaj włoszczyznę i przyprawy, gotuj na małym ogniu 1,5 godziny.",
                "Ugotuj makaron osobno.",
                "Podawaj rosół z makaronem i natką pietruszki."
            }
        },
        new Recipe
        {
            Id = 6,
            Title = "Zupa pomidorowa",
            ImageUrl = "/images/pomidorowa.svg",
            PrepTimeMinutes = 40,
            CategoryId = 3,
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "passata pomidorowa", Amount = 500, Unit = "ml" },
                new Ingredient { Name = "bulion warzywny", Amount = 1, Unit = "l" },
                new Ingredient { Name = "śmietana", Amount = 200, Unit = "ml" },
                new Ingredient { Name = "ryż", Amount = 100, Unit = "g" },
                new Ingredient { Name = "sól", Amount = 1, Unit = "łyżeczka" },
                new Ingredient { Name = "pieprz", Amount = 1, Unit = "szczypta" },
                new Ingredient { Name = "cukier", Amount = 1, Unit = "łyżeczka" }
            },
            Steps = new List<string>
            {
                "Zagotuj bulion.",
                "Dodaj passatę i gotuj kilka minut.",
                "Zahartuj śmietanę i wmieszaj do zupy.",
                "Dopraw i podawaj z ryżem."
            }
        },
        new Recipe
        {
            Id = 7,
            Title = "Sernik tradycyjny",
            ImageUrl = "/images/sernik.svg",
            PrepTimeMinutes = 80,
            CategoryId = 4,
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "twaróg", Amount = 1, Unit = "kg" },
                new Ingredient { Name = "jajka", Amount = 5, Unit = "szt." },
                new Ingredient { Name = "cukier", Amount = 1, Unit = "szklanka" },
                new Ingredient { Name = "masło", Amount = 200, Unit = "g" },
                new Ingredient { Name = "budyń waniliowy", Amount = 1, Unit = "szt." },
                new Ingredient { Name = "spód z ciastek", Amount = 1, Unit = "szt." }
            },
            Steps = new List<string>
            {
                "Zmiksuj twaróg z masłem i cukrem.",
                "Dodawaj jajka i budyń, ciągle miksując.",
                "Wylej masę na spód.",
                "Piecz około 60 minut w 170°C."
            }
        },
        new Recipe
        {
            Id = 8,
            Title = "Szarlotka",
            ImageUrl = "/images/szarlotka.svg",
            PrepTimeMinutes = 70,
            CategoryId = 4,
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "mąka", Amount = 3, Unit = "szklanka" },
                new Ingredient { Name = "masło", Amount = 200, Unit = "g" },
                new Ingredient { Name = "cukier", Amount = 1, Unit = "szklanka" },
                new Ingredient { Name = "jajka", Amount = 1, Unit = "szt." },
                new Ingredient { Name = "proszek do pieczenia", Amount = 1, Unit = "łyżeczka" },
                new Ingredient { Name = "jabłka", Amount = 1, Unit = "kg" },
                new Ingredient { Name = "cynamon", Amount = 1, Unit = "łyżeczka" }
            },
            Steps = new List<string>
            {
                "Zagnieć kruche ciasto i schłodź w lodówce.",
                "Zetrzyj jabłka i poddusz z cynamonem.",
                "Wyłóż połowę ciasta, dodaj jabłka i przykryj resztą ciasta.",
                "Piecz około 50 minut w 180°C."
            }
        }
    };

    // Metoda pomocnicza: dla każdego przepisu ustawia obiekt kategorii na podstawie CategoryId.
    private void PrzypiszKategorie(IEnumerable<Recipe> recipes)
    {
        foreach (var r in recipes)
        {
            r.Category = Categories.FirstOrDefault(c => c.Id == r.CategoryId);
        }
    }

    // Strona główna — lista wszystkich przepisów w formie kart.
    public IActionResult Index()
    {
        PrzypiszKategorie(Recipes);       // dopisz kategorie do przepisów
        ViewBag.Categories = Categories;  // lista kategorii potrzebna do przycisków filtra
        return View(Recipes);             // przekaż listę przepisów do widoku
    }

    // Widok pojedynczego przepisu — pełna karta ze składnikami i krokami.
    public IActionResult Details(int id)
    {
        var recipe = Recipes.FirstOrDefault(r => r.Id == id);
        if (recipe == null)
        {
            return NotFound();
        }

        recipe.Category = Categories.FirstOrDefault(c => c.Id == recipe.CategoryId);
        return View(recipe);
    }

    // Formularz dodawania nowego przepisu.
    // UWAGA: obsługa wysyłki (zapis) jest po stronie JavaScript i tylko na ekranie —
    // trwałe zapisywanie do bazy dorobi backend.
    public IActionResult Add()
    {
        ViewBag.Categories = Categories;        // kategorie do listy rozwijanej
        ViewBag.AllIngredients = AllIngredients; // dostępne składniki
        ViewBag.Units = Units;                   // dostępne jednostki
        return View();
    }

    // "Co mam w lodówce" — użytkownik zaznacza składniki, a JavaScript pokazuje pasujące przepisy.
    public IActionResult Fridge()
    {
        PrzypiszKategorie(Recipes);
        ViewBag.AllIngredients = AllIngredients; // składniki do zaznaczenia
        return View(Recipes);                    // przepisy do przeszukania po stronie przeglądarki
    }

    // Prosta strona informacyjna o serwisie.
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
