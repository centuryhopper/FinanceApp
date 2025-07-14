using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Category
{
    public int Categoryid { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Budgetcap> Budgetcaps { get; set; } = new List<Budgetcap>();

    public virtual ICollection<Streamlinedtransaction> Streamlinedtransactions { get; set; } = new List<Streamlinedtransaction>();
}
