using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Flight
{
    public int Id { get; set; }

    public int? FlightNumber { get; set; }

    public int IdOrigin { get; set; }

    public int IdDestination { get; set; }

    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(16, ErrorMessage = "Máximo 16 caracteres.")]
    public string DepartureTime { get; set; } = null!;
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(16, ErrorMessage = "Máximo 16 caracteres.")]
    public string ArrivalTime { get; set; } = null!;

    public int IdAircraft { get; set; }

    public int IdClass { get; set; }

    public int Periocity { get; set; }

    public virtual ICollection<Function> Functions { get; set; } = new List<Function>();

    public virtual Aircraft IdAircraftNavigation { get; set; } = null!;

    public virtual Class IdClassNavigation { get; set; } = null!;

    public virtual Location IdDestinationNavigation { get; set; } = null!;

    public virtual Location IdOriginNavigation { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
