using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Flight
{
    public int Id { get; set; }

    public string? FlightNumber { get; set; }

    public int IdOrigin { get; set; }

    public int IdDestination { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(10, ErrorMessage = "O número do voo pode ter no máximo 16 caracteres.")]
    public string DepartureTime { get; set; } = null!;
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(10, ErrorMessage = "O número do voo pode ter no máximo 16 caracteres.")]
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
