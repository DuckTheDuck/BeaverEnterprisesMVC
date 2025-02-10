using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Function
{
    public int Id { get; set; }

    public int IdEmployee { get; set; }

    public int IdFlight { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
    public string FunctionDescription { get; set; } = null!;

    public virtual Employee IdEmployeeNavigation { get; set; } = null!;

    public virtual Flight IdFlightNavigation { get; set; } = null!;
}
