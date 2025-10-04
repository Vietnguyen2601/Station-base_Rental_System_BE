using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class AccountRole
{
    public Guid AccountRoleId { get; set; }

    public Guid AccountId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
