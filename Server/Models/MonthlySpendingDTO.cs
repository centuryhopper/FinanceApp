
namespace Server.Models;

public class CategorySum
{
    public decimal Total { get; set; }
    public string Category { get; set; }
}

public class MonthlySpendingDTO
{
    public string Month { get; set; }
    public CategorySum CategorySum { get; set; }
}

