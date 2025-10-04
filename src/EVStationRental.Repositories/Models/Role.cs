using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Role
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
}
