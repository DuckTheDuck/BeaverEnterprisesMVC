using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Passenger
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Surname { get; set; }

    public string? Gender { get; set; }

    public int? SeatNumber { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
