using System;
using System.Collections.Generic;

namespace Server.Models;

public class PlaidItemDTO
{
    public int Plaiditemid { get; set; }

    public int? Userid { get; set; }

    public string Accesstoken { get; set; } = null!;

    public string? Institutionname { get; set; }

    public DateTime? Datelinked { get; set; }
}
