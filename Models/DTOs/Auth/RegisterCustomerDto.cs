using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Auth;

public class RegisterCustomerDto
{
    [Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; set; } = "N/A";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
