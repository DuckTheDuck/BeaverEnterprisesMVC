using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public decimal? MonthlySalary { get; set; }

    public bool IsPilot { get; set; }

    public int? NumHours { get; set; }

    public string License { get; set; } = null!;

    public int? LicenseNum { get; set; }

    public virtual ICollection<Function> Functions { get; set; } = new List<Function>();
}
