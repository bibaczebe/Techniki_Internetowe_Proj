using System.ComponentModel.DataAnnotations;

namespace Techniki_Internetowe_Proj.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Podaj adres email.")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy adres email.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj hasło.")]
    [DataType(DataType.Password)]
    [Display(Name = "Hasło")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Zapamiętaj mnie")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
