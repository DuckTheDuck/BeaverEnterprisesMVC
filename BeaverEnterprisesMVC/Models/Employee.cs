using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Employee
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Name { get; set; } = null!;

    [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
    public string? Address { get; set; }

    [StringLength(15, ErrorMessage = "Máximo 15 caracteres.")]
    public string? Phone { get; set; }

    public decimal? MonthlySalary { get; set; }

    public bool IsPilot { get; set; }

    public int? NumHours { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
    public string License { get; set; } = null!;

    public int? LicenseNum { get; set; }

    public virtual ICollection<Function> Functions { get; set; } = new List<Function>();
}
