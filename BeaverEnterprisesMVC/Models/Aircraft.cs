using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Aircraft
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? IdManufacturer { get; set; }

    public string? Model { get; set; }

    public int Capacity { get; set; }

    public string MinimumLicense { get; set; } = null!;

    public string SerialNumber { get; set; } = null!;

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual Manufacturer? IdManufacturerNavigation { get; set; }
}
