using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Auth;

public class LoginViewModel
{
    [Required]
    [DisplayName("Email Address")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}
