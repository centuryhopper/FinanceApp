using System;
using System.Collections.Generic;

namespace Server.Models;

public class CategoryDTO
{
    public int Categoryid { get; set; }

    public string? Name { get; set; }

    public int? CategoryBudget { get; set; }
}
