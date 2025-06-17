using System;
using System.Collections.Generic;

namespace Server.Entities;

public partial class Log
{
    public int LogId { get; set; }

    public DateOnly DateLogged { get; set; }

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;
}
