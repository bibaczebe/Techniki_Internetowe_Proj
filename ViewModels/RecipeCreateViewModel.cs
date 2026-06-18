using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Techniki_Internetowe_Proj.ViewModels
{
    public class RecipeCreateViewModel
    {
        [Required(ErrorMessage = "Nazwa dania jest absolutnie wymagana.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nazwa musi mieć od 3 do 100 znaków.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Musisz wybrać kategorię.")]
        public string? Category { get; set; }

        [Range(1, 1440, ErrorMessage = "Czas przygotowania musi być realny (od 1 minuty do doby).")]
        public int PrepTime { get; set; }

        [Required(ErrorMessage = "Zdjęcie jest wymagane.")]
        public IFormFile? ImageFile { get; set; }
    }
}