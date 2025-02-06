using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Aircraft
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O número do voo é obrigatório.")]
    [StringLength(10, ErrorMessage = "O número do voo pode ter no máximo 10 caracteres.")]
    public string Name { get; set; } = null!;

    public int? IdManufacturer { get; set; }

    public string? Model { get; set; }

    public int Capacity { get; set; }

    public string MinimumLicense { get; set; } = null!;

    public string SerialNumber { get; set; } = null!;

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();

    public virtual Manufacturer? IdManufacturerNavigation { get; set; }
}
