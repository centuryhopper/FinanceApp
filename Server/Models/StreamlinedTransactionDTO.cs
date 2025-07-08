namespace Server.Models;


public class StreamlinedTransactionDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TransactionId { get; set; }
    public string Name { get; set; }
    public string? Category { get; set; }
    public string? Note { get; set; }
    public decimal Amount { get; set; }
    public DateOnly? Date { get; set; }
    // i.e. sandbox or production
    public string EnvironmentType { get; set; }
}
