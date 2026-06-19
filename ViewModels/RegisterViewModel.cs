using System.ComponentModel.DataAnnotations;

namespace Techniki_Internetowe_Proj.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Podaj adres email.")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy adres email.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podaj hasło.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Hasło musi mieć co najmniej {2} znaków.")]
    [DataType(DataType.Password)]
    [Display(Name = "Hasło")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Powtórz hasło")]
    [Compare(nameof(Password), ErrorMessage = "Hasła nie są takie same.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
