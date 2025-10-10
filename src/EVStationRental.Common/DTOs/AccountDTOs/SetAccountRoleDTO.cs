using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EVStationRental.Common.DTOs
{
    public class SetAccountRoleDTO
    {
        [Required(ErrorMessage = "Account ID is required")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "At least one role is required")]
        [MinLength(1, ErrorMessage = "At least one role must be specified")]
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}
