namespace Techniki_Internetowe_Proj.Models
{
    // Model kategorii przepisu (np. Śniadania, Obiady, Zupy, Desery).
    // To prosta klasa danych — backend później podłączy ją do bazy przez EF Core.
    public class Category
    {
        public int Id { get; set; }            // identyfikator kategorii
        public string Name { get; set; } = ""; // nazwa kategorii widoczna dla użytkownika
    }
}
