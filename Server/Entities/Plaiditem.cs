using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Plaiditem
{
    public int Plaiditemid { get; set; }

    public int? Userid { get; set; }

    public string Accesstoken { get; set; } = null!;

    public string? Institutionname { get; set; }

    public DateTime? Datelinked { get; set; }

    public string? AccessTokenIv { get; set; }

    public string? TransactionsCursor { get; set; }

    public virtual User? User { get; set; }
}
