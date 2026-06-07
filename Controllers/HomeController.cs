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
            Ingredients = new List<string>
            {
                "2 jajka", "1 szklanka mąki", "1 szklanka mleka",
                "szczypta soli", "olej do smażenia", "dżem do podania"
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
            Ingredients = new List<string>
            {
                "3 jajka", "łyżka masła", "sól", "pieprz", "szczypiorek"
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
            Ingredients = new List<string>
            {
                "400 g makaronu spaghetti", "400 g mięsa mielonego",
                "1 cebula", "2 ząbki czosnku", "passata pomidorowa", "oregano", "parmezan"
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
            Ingredients = new List<string>
            {
                "4 plastry schabu", "2 jajka", "bułka tarta", "mąka",
                "sól", "pieprz", "ziemniaki", "olej do smażenia"
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
            Ingredients = new List<string>
            {
                "1 kg mięsa na rosół", "włoszczyzna", "2 liście laurowe",
                "ziele angielskie", "sól", "makaron nitki", "natka pietruszki"
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
            Ingredients = new List<string>
            {
                "passata pomidorowa", "bulion warzywny", "śmietana",
                "ryż lub makaron", "sól", "pieprz", "cukier"
            },
            Steps = new List<string>
            {
                "Zagotuj bulion.",
                "Dodaj passatę i gotuj kilka minut.",
                "Zahartuj śmietanę i wmieszaj do zupy.",
                "Dopraw i podawaj z ryżem lub makaronem."
            }
        },
        new Recipe
        {
            Id = 7,
            Title = "Sernik tradycyjny",
            ImageUrl = "/images/sernik.svg",
            PrepTimeMinutes = 80,
            CategoryId = 4,
            Ingredients = new List<string>
            {
                "1 kg twarogu", "5 jajek", "1 szklanka cukru",
                "kostka masła", "budyń waniliowy", "spód z ciastek"
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
            Ingredients = new List<string>
            {
                "3 szklanki mąki", "kostka masła", "1 szklanka cukru",
                "1 jajko", "proszek do pieczenia", "1 kg jabłek", "cynamon"
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
    // Dzięki temu w widoku możemy wygodnie pokazać nazwę kategorii.
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
        // Szukamy przepisu o podanym Id na liście tymczasowej
        var recipe = Recipes.FirstOrDefault(r => r.Id == id);

        // Jeśli nie ma takiego przepisu — zwracamy stronę 404
        if (recipe == null)
        {
            return NotFound();
        }

        // Uzupełniamy kategorię, żeby pokazać jej nazwę w widoku
        recipe.Category = Categories.FirstOrDefault(c => c.Id == recipe.CategoryId);
        return View(recipe);
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
