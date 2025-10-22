using System.ComponentModel.DataAnnotations;

namespace EVStationRental.Common.DTOs.Authentication;

public class RefreshTokenRequestDTO
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}


