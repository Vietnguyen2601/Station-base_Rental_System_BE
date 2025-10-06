using System.ComponentModel.DataAnnotations;

namespace EVStationRental.Common.DTOs.Authentication;

public class RegisterRequestDTO
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Invalid username characters")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$", ErrorMessage = "Password must include upper, lower, digit, special")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Password and ConfirmPassword do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Invalid contact number")]
    public string? ContactNumber { get; set; }
}


