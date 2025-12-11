namespace Shared.Models;

public class CurrentMonthSpendingByCategoryDTO
{
    public int Id { get; set; }
    public string Category { get; set; } = null!;
    public int CategoryId { get; set; }
    public decimal Spent { get; set; }
    public int CategoryBudget { get; set; }
}