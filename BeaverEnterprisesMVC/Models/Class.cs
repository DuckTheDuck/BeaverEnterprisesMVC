using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Class
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Name { get; set; } = null!;

    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string? Description { get; set; }

    public int Capacity { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
