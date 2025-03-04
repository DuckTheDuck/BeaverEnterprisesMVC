using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Account
{
    public int Id { get; set; }

    public int Code { get; set; }

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
