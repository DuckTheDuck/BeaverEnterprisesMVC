using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Location
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<Flight> FlightIdDestinationNavigations { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightIdOriginNavigations { get; set; } = new List<Flight>();
}
