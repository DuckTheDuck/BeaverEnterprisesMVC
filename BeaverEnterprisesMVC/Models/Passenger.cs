using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Passenger
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public int? Nif { get; set; }

    public int? Code { get; set; }

    public string Gmail { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
