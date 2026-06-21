using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Techniki_Internetowe_Proj.Models;

namespace Techniki_Internetowe_Proj.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _env;

    // Blokada przy dopisywaniu do współdzielonej (statycznej) listy przepisów.
    private static readonly object _recipesLock = new object();

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    // ================================================================
    //  DANE TYMCZASOWE (NA SZTYWNO)
    //  UWAGA: poniższe listy to dane przykładowe trzymane w pamięci.
    //  Służą TYLKO do zbudowania i pokazania frontendu.
    //  Backend (EF Core / baza danych) PODMIENI je później na dane z bazy.
    //  Bazę składników (do podpowiedzi) trzymamy w wwwroot/data/ingredients.json.
    // ================================================================

    // Twardo zdefiniowana lista jednostek miary (używana w formularzu dodawania).
    private static readonly List<string> Units = new List<string>
    {
        "szt.", "g", "dag", "kg", "ml", "l", "szklanka", "łyżka", "łyżeczka", "szczypta", "ząbek"
    };

    // Lista kategorii
    private static readonly List<Category> Categories = new List<Category>
    {
        new Category { Id = 1, Name = "Śniadania" },
        new Category { Id = 2, Name = "Obiady" },
        new Category { Id = 3, Name = "Zupy" },
        new Category { Id = 4, Name = "Desery" },
        new Category { Id = 5, Name = "Sałatki" },
        new Category { Id = 6, Name = "Kolacje" },
        new Category { Id = 7, Name = "Napoje" },
        new Category { Id = 8, Name = "Przekąski" }
    };

    // Krótka metoda pomocnicza, żeby zwięźle tworzyć składniki.
    private static Ingredient Skl(string name, double amount, string unit)
    {
        return new Ingredient { Name = name, Amount = amount, Unit = unit };
    }

    // Lista przepisów (na sztywno — backend podmieni na bazę)
    private static readonly List<Recipe> Recipes = new List<Recipe>
    {
        new Recipe
        {
            Id = 1, Title = "Naleśniki z dżemem", ImageUrl = "/images/nalesniki.svg",
            PrepTimeMinutes = 25, CategoryId = 1,
            Ingredients = new List<Ingredient>
            {
                Skl("jajka", 2, "szt."), Skl("mąka", 1, "szklanka"), Skl("mleko", 1, "szklanka"),
                Skl("sól", 1, "szczypta"), Skl("olej", 1, "łyżka"), Skl("dżem", 3, "łyżka")
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
            Id = 2, Title = "Jajecznica na maśle", ImageUrl = "/images/jajecznica.svg",
            PrepTimeMinutes = 10, CategoryId = 1,
            Ingredients = new List<Ingredient>
            {
                Skl("jajka", 3, "szt."), Skl("masło", 1, "łyżka"), Skl("sól", 1, "szczypta"),
                Skl("pieprz", 1, "szczypta"), Skl("szczypiorek", 1, "łyżka")
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
            Id = 3, Title = "Owsianka z owocami", ImageUrl = "/images/owsianka.svg",
            PrepTimeMinutes = 10, CategoryId = 1,
            Ingredients = new List<Ingredient>
            {
                Skl("płatki owsiane", 1, "szklanka"), Skl("mleko", 1, "szklanka"),
                Skl("banan", 1, "szt."), Skl("miód", 1, "łyżka"), Skl("borówki", 50, "g")
            },
            Steps = new List<string>
            {
                "Zagotuj mleko i wsyp płatki owsiane.",
                "Gotuj kilka minut, aż zgęstnieje.",
                "Dodaj pokrojonego banana, borówki i miód."
            }
        },
        new Recipe
        {
            Id = 4, Title = "Spaghetti Bolognese", ImageUrl = "/images/spaghetti.svg",
            PrepTimeMinutes = 45, CategoryId = 2,
            Ingredients = new List<Ingredient>
            {
                Skl("makaron spaghetti", 400, "g"), Skl("mięso mielone", 400, "g"),
                Skl("cebula", 1, "szt."), Skl("czosnek", 2, "ząbek"),
                Skl("passata pomidorowa", 500, "ml"), Skl("oregano", 1, "łyżeczka"),
                Skl("parmezan", 50, "g")
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
            Id = 5, Title = "Kotlet schabowy z ziemniakami", ImageUrl = "/images/schabowy.svg",
            PrepTimeMinutes = 60, CategoryId = 2,
            Ingredients = new List<Ingredient>
            {
                Skl("schab", 4, "szt."), Skl("jajka", 2, "szt."), Skl("bułka tarta", 1, "szklanka"),
                Skl("mąka", 3, "łyżka"), Skl("sól", 1, "szczypta"), Skl("pieprz", 1, "szczypta"),
                Skl("ziemniaki", 1, "kg"), Skl("olej", 100, "ml")
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
            Id = 6, Title = "Pierogi ruskie", ImageUrl = "/images/pierogi.svg",
            PrepTimeMinutes = 75, CategoryId = 2,
            Ingredients = new List<Ingredient>
            {
                Skl("mąka", 3, "szklanka"), Skl("woda", 1, "szklanka"), Skl("ziemniaki", 0.5, "kg"),
                Skl("twaróg", 250, "g"), Skl("cebula", 1, "szt."), Skl("sól", 1, "łyżeczka"),
                Skl("pieprz", 1, "szczypta")
            },
            Steps = new List<string>
            {
                "Zagnieć ciasto z mąki, wody i szczypty soli.",
                "Ugotuj ziemniaki i wymieszaj z twarogiem oraz podsmażoną cebulą.",
                "Lep pierogi i gotuj w osolonej wodzie do wypłynięcia.",
                "Podawaj okraszone cebulką."
            }
        },
        new Recipe
        {
            Id = 7, Title = "Rosół z makaronem", ImageUrl = "/images/rosol.svg",
            PrepTimeMinutes = 90, CategoryId = 3,
            Ingredients = new List<Ingredient>
            {
                Skl("mięso na rosół", 1, "kg"), Skl("włoszczyzna", 1, "szt."),
                Skl("liść laurowy", 2, "szt."), Skl("ziele angielskie", 4, "szt."),
                Skl("sól", 2, "łyżeczka"), Skl("makaron", 200, "g"), Skl("natka pietruszki", 1, "łyżka")
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
            Id = 8, Title = "Zupa pomidorowa", ImageUrl = "/images/pomidorowa.svg",
            PrepTimeMinutes = 40, CategoryId = 3,
            Ingredients = new List<Ingredient>
            {
                Skl("passata pomidorowa", 500, "ml"), Skl("bulion warzywny", 1, "l"),
                Skl("śmietana", 200, "ml"), Skl("ryż", 100, "g"), Skl("sól", 1, "łyżeczka"),
                Skl("pieprz", 1, "szczypta"), Skl("cukier", 1, "łyżeczka")
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
            Id = 9, Title = "Żurek staropolski", ImageUrl = "/images/zurek.svg",
            PrepTimeMinutes = 60, CategoryId = 3,
            Ingredients = new List<Ingredient>
            {
                Skl("zakwas na żurek", 500, "ml"), Skl("kiełbasa biała", 300, "g"),
                Skl("jajka", 2, "szt."), Skl("ziemniaki", 0.5, "kg"), Skl("czosnek", 2, "ząbek"),
                Skl("majeranek", 1, "łyżeczka"), Skl("śmietana", 100, "ml")
            },
            Steps = new List<string>
            {
                "Ugotuj kiełbasę w wodzie z czosnkiem.",
                "Wlej zakwas i zagotuj, mieszając.",
                "Dopraw majerankiem i zabiel śmietaną.",
                "Podawaj z jajkiem i ziemniakami."
            }
        },
        new Recipe
        {
            Id = 10, Title = "Sernik tradycyjny", ImageUrl = "/images/sernik.svg",
            PrepTimeMinutes = 80, CategoryId = 4,
            Ingredients = new List<Ingredient>
            {
                Skl("twaróg", 1, "kg"), Skl("jajka", 5, "szt."), Skl("cukier", 1, "szklanka"),
                Skl("masło", 200, "g"), Skl("budyń waniliowy", 1, "szt."), Skl("spód z ciastek", 1, "szt.")
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
            Id = 11, Title = "Szarlotka", ImageUrl = "/images/szarlotka.svg",
            PrepTimeMinutes = 70, CategoryId = 4,
            Ingredients = new List<Ingredient>
            {
                Skl("mąka", 3, "szklanka"), Skl("masło", 200, "g"), Skl("cukier", 1, "szklanka"),
                Skl("jajka", 1, "szt."), Skl("proszek do pieczenia", 1, "łyżeczka"),
                Skl("jabłka", 1, "kg"), Skl("cynamon", 1, "łyżeczka")
            },
            Steps = new List<string>
            {
                "Zagnieć kruche ciasto i schłodź w lodówce.",
                "Zetrzyj jabłka i poddusz z cynamonem.",
                "Wyłóż połowę ciasta, dodaj jabłka i przykryj resztą ciasta.",
                "Piecz około 50 minut w 180°C."
            }
        },
        new Recipe
        {
            Id = 12, Title = "Brownie czekoladowe", ImageUrl = "/images/brownie.svg",
            PrepTimeMinutes = 50, CategoryId = 4,
            Ingredients = new List<Ingredient>
            {
                Skl("czekolada gorzka", 200, "g"), Skl("masło", 150, "g"), Skl("cukier", 1, "szklanka"),
                Skl("jajka", 3, "szt."), Skl("mąka", 1, "szklanka"), Skl("kakao", 2, "łyżka")
            },
            Steps = new List<string>
            {
                "Rozpuść czekoladę z masłem.",
                "Dodaj cukier, jajka, mąkę i kakao, wymieszaj.",
                "Przelej do formy.",
                "Piecz około 30 minut w 180°C."
            }
        },
        new Recipe
        {
            Id = 13, Title = "Sałatka grecka", ImageUrl = "/images/salatka-grecka.svg",
            PrepTimeMinutes = 15, CategoryId = 5,
            Ingredients = new List<Ingredient>
            {
                Skl("pomidory", 3, "szt."), Skl("ogórek", 1, "szt."), Skl("ser feta", 200, "g"),
                Skl("oliwki", 100, "g"), Skl("cebula czerwona", 1, "szt."),
                Skl("oliwa z oliwek", 3, "łyżka"), Skl("oregano", 1, "łyżeczka")
            },
            Steps = new List<string>
            {
                "Pokrój pomidory, ogórka i cebulę.",
                "Dodaj pokrojoną fetę i oliwki.",
                "Skrop oliwą i posyp oregano."
            }
        },
        new Recipe
        {
            Id = 14, Title = "Sałatka jarzynowa", ImageUrl = "/images/salatka-jarzynowa.svg",
            PrepTimeMinutes = 40, CategoryId = 5,
            Ingredients = new List<Ingredient>
            {
                Skl("ziemniaki", 0.5, "kg"), Skl("marchew", 3, "szt."), Skl("groszek konserwowy", 1, "szt."),
                Skl("ogórek kiszony", 3, "szt."), Skl("jajka", 3, "szt."), Skl("majonez", 4, "łyżka"),
                Skl("sól", 1, "łyżeczka")
            },
            Steps = new List<string>
            {
                "Ugotuj ziemniaki, marchew i jajka, ostudź.",
                "Pokrój wszystko w drobną kostkę.",
                "Dodaj groszek, ogórki i majonez.",
                "Wymieszaj i dopraw solą."
            }
        },
        new Recipe
        {
            Id = 15, Title = "Sałatka z tuńczykiem", ImageUrl = "/images/salatka-tunczyk.svg",
            PrepTimeMinutes = 15, CategoryId = 5,
            Ingredients = new List<Ingredient>
            {
                Skl("tuńczyk", 1, "szt."), Skl("kukurydza", 1, "szt."), Skl("sałata", 1, "szt."),
                Skl("pomidory", 2, "szt."), Skl("majonez", 2, "łyżka"), Skl("sól", 1, "szczypta")
            },
            Steps = new List<string>
            {
                "Porwij sałatę i ułóż w misce.",
                "Dodaj tuńczyka, kukurydzę i pokrojone pomidory.",
                "Wymieszaj z majonezem i dopraw."
            }
        },
        new Recipe
        {
            Id = 16, Title = "Kanapki z serem i szynką", ImageUrl = "/images/kanapki.svg",
            PrepTimeMinutes = 10, CategoryId = 6,
            Ingredients = new List<Ingredient>
            {
                Skl("chleb", 4, "szt."), Skl("masło", 1, "łyżka"), Skl("ser żółty", 4, "szt."),
                Skl("szynka", 4, "szt."), Skl("pomidory", 1, "szt."), Skl("sałata", 1, "szt.")
            },
            Steps = new List<string>
            {
                "Posmaruj chleb masłem.",
                "Ułóż ser, szynkę i plasterki pomidora.",
                "Dodaj liść sałaty i przykryj kromką."
            }
        },
        new Recipe
        {
            Id = 17, Title = "Tosty z serem", ImageUrl = "/images/tosty.svg",
            PrepTimeMinutes = 10, CategoryId = 6,
            Ingredients = new List<Ingredient>
            {
                Skl("chleb tostowy", 4, "szt."), Skl("ser żółty", 4, "szt."),
                Skl("szynka", 2, "szt."), Skl("masło", 1, "łyżka")
            },
            Steps = new List<string>
            {
                "Posmaruj chleb masłem.",
                "Włóż ser i szynkę między kromki.",
                "Zapiekaj w opiekaczu do zrumienienia."
            }
        },
        new Recipe
        {
            Id = 18, Title = "Omlet z warzywami", ImageUrl = "/images/omlet.svg",
            PrepTimeMinutes = 15, CategoryId = 6,
            Ingredients = new List<Ingredient>
            {
                Skl("jajka", 3, "szt."), Skl("papryka", 1, "szt."), Skl("pomidory", 1, "szt."),
                Skl("szczypiorek", 1, "łyżka"), Skl("sól", 1, "szczypta"), Skl("olej", 1, "łyżka")
            },
            Steps = new List<string>
            {
                "Roztrzep jajka z solą.",
                "Wlej na patelnię z olejem.",
                "Dodaj pokrojoną paprykę i pomidora.",
                "Smaż do ścięcia i posyp szczypiorkiem."
            }
        },
        new Recipe
        {
            Id = 19, Title = "Koktajl bananowy", ImageUrl = "/images/koktajl.svg",
            PrepTimeMinutes = 5, CategoryId = 7,
            Ingredients = new List<Ingredient>
            {
                Skl("banan", 2, "szt."), Skl("mleko", 1, "szklanka"),
                Skl("jogurt naturalny", 150, "g"), Skl("miód", 1, "łyżka")
            },
            Steps = new List<string>
            {
                "Obierz banany i włóż do blendera.",
                "Dodaj mleko, jogurt i miód.",
                "Zmiksuj na gładko i podawaj schłodzony."
            }
        },
        new Recipe
        {
            Id = 20, Title = "Lemoniada cytrynowa", ImageUrl = "/images/lemoniada.svg",
            PrepTimeMinutes = 10, CategoryId = 7,
            Ingredients = new List<Ingredient>
            {
                Skl("cytryna", 3, "szt."), Skl("woda", 1, "l"), Skl("cukier", 4, "łyżka"),
                Skl("mięta", 1, "szt."), Skl("lód", 100, "g")
            },
            Steps = new List<string>
            {
                "Wyciśnij sok z cytryn.",
                "Rozpuść cukier w wodzie.",
                "Połącz sok, wodę, miętę i lód."
            }
        },
        new Recipe
        {
            Id = 21, Title = "Herbata imbirowa", ImageUrl = "/images/herbata.svg",
            PrepTimeMinutes = 10, CategoryId = 7,
            Ingredients = new List<Ingredient>
            {
                Skl("imbir", 1, "szt."), Skl("cytryna", 1, "szt."), Skl("miód", 2, "łyżka"),
                Skl("woda", 0.5, "l")
            },
            Steps = new List<string>
            {
                "Zagotuj wodę z plasterkami imbiru.",
                "Dodaj sok z cytryny.",
                "Posłodź miodem i podawaj na ciepło."
            }
        },
        new Recipe
        {
            Id = 22, Title = "Hummus", ImageUrl = "/images/hummus.svg",
            PrepTimeMinutes = 15, CategoryId = 8,
            Ingredients = new List<Ingredient>
            {
                Skl("ciecierzyca", 1, "szt."), Skl("tahini", 2, "łyżka"), Skl("czosnek", 1, "ząbek"),
                Skl("cytryna", 1, "szt."), Skl("oliwa z oliwek", 3, "łyżka"), Skl("kmin rzymski", 1, "łyżeczka")
            },
            Steps = new List<string>
            {
                "Odsącz ciecierzycę.",
                "Zmiksuj z tahini, czosnkiem, sokiem z cytryny i oliwą.",
                "Dopraw kminem i podawaj z pieczywem."
            }
        },
        new Recipe
        {
            Id = 23, Title = "Placki ziemniaczane", ImageUrl = "/images/placki.svg",
            PrepTimeMinutes = 35, CategoryId = 8,
            Ingredients = new List<Ingredient>
            {
                Skl("ziemniaki", 1, "kg"), Skl("cebula", 1, "szt."), Skl("jajka", 1, "szt."),
                Skl("mąka", 3, "łyżka"), Skl("sól", 1, "łyżeczka"), Skl("olej", 100, "ml")
            },
            Steps = new List<string>
            {
                "Zetrzyj ziemniaki i cebulę.",
                "Dodaj jajko, mąkę i sól, wymieszaj.",
                "Smaż placki na rozgrzanym oleju z obu stron."
            }
        },
        new Recipe
        {
            Id = 24, Title = "Frytki z piekarnika", ImageUrl = "/images/frytki.svg",
            PrepTimeMinutes = 40, CategoryId = 8,
            Ingredients = new List<Ingredient>
            {
                Skl("ziemniaki", 1, "kg"), Skl("oliwa z oliwek", 3, "łyżka"), Skl("sól", 1, "łyżeczka"),
                Skl("papryka słodka", 1, "łyżeczka")
            },
            Steps = new List<string>
            {
                "Pokrój ziemniaki w słupki.",
                "Wymieszaj z oliwą, solą i papryką.",
                "Piecz około 30 minut w 200°C, raz przewracając."
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
        PrzypiszKategorie(Recipes);
        ViewBag.Categories = Categories;  // lista kategorii potrzebna do przycisków filtra
        return View(Recipes);
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
    // Składniki dodaje się dynamicznie (JavaScript), wpisując nazwę z podpowiedziami (datalist).
    // [Authorize] => tę stronę widzą TYLKO zalogowani; gość zostaje przekierowany na logowanie.
    [Authorize]
    public IActionResult Add()
    {
        ViewBag.Categories = Categories; // kategorie do listy rozwijanej
        ViewBag.Units = Units;           // dostępne jednostki
        return View();
    }

    // Zapisanie nowego przepisu wysłanego z formularza.
    // Przepis trafia do współdzielonej listy, więc OD RAZU widać go na stronie głównej.
    // Składniki przychodzą jako trzy równoległe tablice (nazwa / ilość / jednostka).
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
        string Title,
        string Category,
        int PrepTime,
        string[]? IngredientNames,
        string[]? IngredientAmounts,
        string[]? IngredientUnits,
        string? Steps,
        IFormFile? imageFile)
    {
        // Minimalna walidacja po stronie serwera (nazwa min. 3 znaki, realny czas).
        if (string.IsNullOrWhiteSpace(Title) || Title.Trim().Length < 3)
        {
            ModelState.AddModelError(string.Empty, "Podaj nazwę dania (przynajmniej 3 znaki).");
        }
        if (PrepTime < 1)
        {
            ModelState.AddModelError(string.Empty, "Podaj poprawny czas przygotowania.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = Categories;
            ViewBag.Units = Units;
            return View();
        }

        // Składniki: łączymy trzy tablice po indeksie, pomijając puste nazwy.
        var ingredients = new List<Ingredient>();
        if (IngredientNames != null)
        {
            for (int i = 0; i < IngredientNames.Length; i++)
            {
                var name = IngredientNames[i]?.Trim();
                if (string.IsNullOrEmpty(name)) continue;

                // Ilość: akceptujemy kropkę i przecinek; gdy się nie uda — domyślnie 1.
                double amount = 1;
                if (IngredientAmounts != null && i < IngredientAmounts.Length)
                {
                    var raw = (IngredientAmounts[i] ?? "").Replace(',', '.');
                    if (!double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out amount) || amount <= 0)
                    {
                        amount = 1;
                    }
                }

                var unit = (IngredientUnits != null && i < IngredientUnits.Length)
                    ? (IngredientUnits[i] ?? "")
                    : "";

                ingredients.Add(new Ingredient { Name = name, Amount = amount, Unit = unit });
            }
        }

        // Kroki: każda niepusta linia z pola tekstowego to osobny krok.
        var steps = (Steps ?? "")
            .Replace("\r\n", "\n")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        // Kategoria przychodzi jako nazwa — zamieniamy ją na obiekt/Id (gdy brak: pierwsza).
        var category = Categories.FirstOrDefault(c => c.Name == Category) ?? Categories.First();

        // Zdjęcie: jeśli wgrano plik, zapisujemy go w wwwroot/images; inaczej placeholder.
        string imageUrl = "/images/placeholder.svg";
        if (imageFile != null && imageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            imageUrl = "/images/" + fileName;
        }

        // Dopisujemy przepis do listy (z blokadą — lista jest współdzielona).
        lock (_recipesLock)
        {
            int newId = Recipes.Count > 0 ? Recipes.Max(r => r.Id) + 1 : 1;
            Recipes.Add(new Recipe
            {
                Id = newId,
                Title = Title.Trim(),
                ImageUrl = imageUrl,
                PrepTimeMinutes = PrepTime,
                CategoryId = category.Id,
                Category = category,
                Ingredients = ingredients,
                Steps = steps
            });
        }

        // Komunikat dla strony głównej + przekierowanie tam, żeby od razu zobaczyć nowy przepis.
        TempData["Added"] = $"Dodano przepis: {Title.Trim()}";
        return RedirectToAction(nameof(Index));
    }

    // "Co mam w lodówce" — użytkownik wpisuje swoje składniki, a JavaScript pokazuje pasujące przepisy.
    public IActionResult Fridge()
    {
        PrzypiszKategorie(Recipes);
        return View(Recipes); // przepisy do przeszukania po stronie przeglądarki
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
