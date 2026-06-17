namespace Techniki_Internetowe_Proj.Models
{
    // Model pojedynczego przepisu kulinarnego.
    // To prosta klasa danych dla widoków — backend później podłączy ją do bazy (EF Core).
    public class Recipe
    {
        public int Id { get; set; }                // identyfikator przepisu
        public string Title { get; set; } = "";    // tytuł przepisu
        public string ImageUrl { get; set; } = ""; // ścieżka do zdjęcia (plik w wwwroot/images)

        // Lista składników — każdy składnik ma nazwę, ilość i jednostkę (np. 200 g mąki)
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        // Kroki przygotowania — każdy krok to osobny tekst (w widoku pokażemy jako listę numerowaną)
        public List<string> Steps { get; set; } = new List<string>();

        public int PrepTimeMinutes { get; set; }   // czas przygotowania w minutach

        // Powiązanie z kategorią: trzymamy Id kategorii oraz (opcjonalnie) sam obiekt kategorii
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
