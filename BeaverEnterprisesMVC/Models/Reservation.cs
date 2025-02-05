using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Reservation
{
    public int Id { get; set; }

    public int IdPassenger { get; set; }

    public int IdFlight { get; set; }

    public bool Confirmed { get; set; }

    public virtual Flight IdFlightNavigation { get; set; } = null!;

    public virtual Passenger IdPassengerNavigation { get; set; } = null!;
}
