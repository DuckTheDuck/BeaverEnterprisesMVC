using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int? IdPassager { get; set; }

    public int? SeatNumber { get; set; }

    public int IdFlightSchedule { get; set; }

    public double? Price { get; set; }

    public string Status { get; set; } = null!;

    public string? Type { get; set; }

    public virtual Flightschedule IdFlightScheduleNavigation { get; set; } = null!;

    public virtual Passenger? IdPassagerNavigation { get; set; }

    public virtual ICollection<Orderbuy> Orderbuys { get; set; } = new List<Orderbuy>();
}
