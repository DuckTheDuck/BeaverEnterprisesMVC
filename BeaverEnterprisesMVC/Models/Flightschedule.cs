using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Flightschedule
{
    public int Id { get; set; }

    public int IdFlight { get; set; }

    public DateOnly FlightDate { get; set; }

    public virtual Flight IdFlightNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
