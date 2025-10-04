using System;
using System.Collections.Generic;

namespace EVStationRental.Repositories.Models;

public partial class Account
{
    public Guid AccountId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<License> Licenses { get; set; } = new List<License>();

    public virtual ICollection<Order> OrderCustomers { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderStaffs { get; set; } = new List<Order>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<StaffRevenue> StaffRevenues { get; set; } = new List<StaffRevenue>();
}
