using System;
using System.ComponentModel.DataAnnotations;

namespace EVStationRental.Common.DTOs
{
    public class SetAdminRoleDTO
    {
        [Required(ErrorMessage = "Account ID is required")]
        public Guid AccountId { get; set; }
    }
}
