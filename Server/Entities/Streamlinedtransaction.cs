using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Streamlinedtransaction
{
    public int Streamlinedtransactionsid { get; set; }

    public int? Userid { get; set; }

    public string? Transactionid { get; set; }

    public string? Name { get; set; }

    public string? Note { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Date { get; set; }

    public string? Environmenttype { get; set; }

    public int? Bankinfoid { get; set; }

    public virtual Bankinfo? Bankinfo { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
