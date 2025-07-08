using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Bankinfo
{
    public int Bankinfoid { get; set; }

    public int? Userid { get; set; }

    public string Bankname { get; set; } = null!;

    public decimal Totalbankbalance { get; set; }

    public virtual ICollection<Streamlinedtransaction> Streamlinedtransactions { get; set; } = new List<Streamlinedtransaction>();

    public virtual User? User { get; set; }
}
