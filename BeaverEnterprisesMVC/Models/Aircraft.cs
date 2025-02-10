using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Aircraft
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Name { get; set; } = null!;

    public int? IdManufacturer { get; set; }

    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string? Model { get; set; }

    public int Capacity { get; set; }

    public string MinimumLicense { get; set; } = null!;

    public string SerialNumber { get; set; } = null!;

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual Manufacturer? IdManufacturerNavigation { get; set; }
}
