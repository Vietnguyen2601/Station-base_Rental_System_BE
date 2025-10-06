using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Common.DTOs
{
    public class AccountDTO
    {
        public Guid AccountId { get; set; }

        public string Username { get; set; } = null!;
    
        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? ContactNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public string AccountRoles { get; set; } = null!;
    }
}
