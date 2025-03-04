using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Order
{
    public int Id { get; set; }

    public int IdAccount { get; set; }

    public DateOnly Date { get; set; }

    public string Status { get; set; } = null!;

    public virtual Account IdAccountNavigation { get; set; } = null!;

    public virtual ICollection<Orderbuy> Orderbuys { get; set; } = new List<Orderbuy>();
}
