using System;
using System.Collections.Generic;

namespace BeaverEnterprisesMVC.Models;

public partial class Orderbuy
{
    public int Id { get; set; }

    public int IdTicket { get; set; }

    public int IdOrder { get; set; }

    public virtual Order IdOrderNavigation { get; set; } = null!;

    public virtual Ticket IdTicketNavigation { get; set; } = null!;
}
