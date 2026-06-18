using System.ComponentModel.DataAnnotations;

namespace Techniki_Internetowe_Proj.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "To nie jest poprawny adres e-mail.")]
        [Display(Name = "Adres e-mail")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło must mieć co najmniej 6 znaków.")]
        [Display(Name = "Hasło")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Podane hasła nie są identyczne.")]
        public string? ConfirmPassword { get; set; }
    }
}