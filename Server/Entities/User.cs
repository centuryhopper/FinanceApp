﻿using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class User
{
    public int Id { get; set; }

    public string UmsUserid { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public DateTime? Datelastlogin { get; set; }

    public DateTime? Datelastlogout { get; set; }

    public DateTime? Datecreated { get; set; }

    public DateTime? Dateretired { get; set; }

    public virtual ICollection<Bankinfo> Bankinfos { get; set; } = new List<Bankinfo>();

    public virtual ICollection<Plaiditem> Plaiditems { get; set; } = new List<Plaiditem>();

    public virtual ICollection<Streamlinedtransaction> Streamlinedtransactions { get; set; } = new List<Streamlinedtransaction>();
}
