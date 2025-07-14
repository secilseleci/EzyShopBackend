using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Auth;

public class RegisterCustomerViewModel
{
    [Required]
    public string FullName { get; set; } = "N/A";
     
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Address")]
    public string Address { get; set; } = null!;

    [Required]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;
}
