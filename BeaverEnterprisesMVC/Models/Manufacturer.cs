using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Manufacturer
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string Name { get; set; } = null!;

    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string? CountryOfOrigin { get; set; }

    public virtual ICollection<Aircraft> Aircraft { get; set; } = new List<Aircraft>();
}
