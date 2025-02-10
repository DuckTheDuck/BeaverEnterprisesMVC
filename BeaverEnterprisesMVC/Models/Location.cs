using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Location
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Name { get; set; } = null!;

    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string? Country { get; set; }

    public virtual ICollection<Flight> FlightIdDestinationNavigations { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightIdOriginNavigations { get; set; } = new List<Flight>();
}
