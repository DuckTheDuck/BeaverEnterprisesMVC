using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int IdTicket { get; set; }

    public int IdAccount { get; set; }

    public string Status { get; set; } = null!;

    public virtual Account IdAccountNavigation { get; set; } = null!;

    public virtual Ticket IdTicketNavigation { get; set; } = null!;
}
