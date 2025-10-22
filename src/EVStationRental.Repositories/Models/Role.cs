using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Role
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public bool Isactive { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
