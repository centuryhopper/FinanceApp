namespace Server.Models;

public class CurrentMonthSpendingByCategoryDTO
{
    public int Id { get; set; }
    public string Category { get; set; } = null!;
    public int BudgetCap { get; set; }
    public decimal Spent { get; set; }
}