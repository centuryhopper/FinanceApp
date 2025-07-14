using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Budgetcap
{
    public int Categorybudget { get; set; }

    public int Categoryid { get; set; }

    public int Bankinfoid { get; set; }

    public int Userid { get; set; }

    public virtual Bankinfo Bankinfo { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
