
using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class BankInfoDTO
{
    public int Bankinfoid { get; set; }

    public int? Userid { get; set; }

    public string Bankname { get; set; } = null!;

    public decimal Totalbankbalance { get; set; }
}


