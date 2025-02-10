using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaverEnterprisesMVC.Models;

public partial class Passenger
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres.")]
    public string Name { get; set; } = null!;
    [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
    public string? Address { get; set; }
    [StringLength(15, ErrorMessage = "Máximo 15 caracteres.")]
    public string? Phone { get; set; }
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
    public string Cc { get; set; } = null!;
    [Required(ErrorMessage = "Este campo é obrigatório.")]
    [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
    public string Nif { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
