using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int IdPassager { get; set; }

    public string? SeatNumber { get; set; }

    public int IdFlightSchedule { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Flightschedule IdFlightScheduleNavigation { get; set; } = null!;

    public virtual Passenger IdPassagerNavigation { get; set; } = null!;
}
