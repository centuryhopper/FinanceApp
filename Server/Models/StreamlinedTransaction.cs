namespace Server.Models;

// TODO: alter the transactions table to include these fields only
// TODO: create an api call that syncs the latest transactions with your application
public class StreamlinedTransactionDTO
{
    public int Id { get; set; }
    public string TransactionId { get; set; }
    public string Name { get; set; }
    public string? Category { get; set; }
    public string? Note { get; set; }
    public decimal Amount { get; set; }
    public DateOnly? Date { get; set; }
}
