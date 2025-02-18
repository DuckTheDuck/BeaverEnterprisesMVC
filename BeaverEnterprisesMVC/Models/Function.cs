using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Function
{
    public int Id { get; set; }

    public int IdEmployee { get; set; }

    public int IdFlight { get; set; }

    public string FunctionDescription { get; set; } = null!;

    public virtual Employee IdEmployeeNavigation { get; set; } = null!;

    public virtual Flight IdFlightNavigation { get; set; } = null!;
}
