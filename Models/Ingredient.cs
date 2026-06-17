namespace Techniki_Internetowe_Proj.Models
{
    // Model pojedynczego składnika przepisu: nazwa + ilość + jednostka.
    // Dzięki temu możemy pokazać np. "200 g mąki" oraz porównywać składniki
    // (potrzebne w wyszukiwarce "co mam w lodówce").
    // To prosta klasa danych — backend później przeniesie ją do bazy (EF Core).
    public class Ingredient
    {
        public string Name { get; set; } = "";   // nazwa składnika, np. "mąka"
        public double Amount { get; set; }        // ilość, np. 200
        public string Unit { get; set; } = "";    // jednostka, np. "g", "łyżeczka", "szt."
    }
}
