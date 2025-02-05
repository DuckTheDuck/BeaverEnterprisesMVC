using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Class
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Capacity { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Aircraft> Aircraft { get; set; } = new List<Aircraft>();
}
