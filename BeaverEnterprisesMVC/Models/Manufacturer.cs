using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? CountryOfOrigin { get; set; }

    public virtual ICollection<Aircraft> Aircraft { get; set; } = new List<Aircraft>();
}
