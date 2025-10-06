using System.ComponentModel.DataAnnotations;

namespace EVStationRental.Common.DTOs.Authentication;

public class LoginRequestDTO
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}


