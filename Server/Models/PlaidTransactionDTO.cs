namespace Server.Models;


public class PlaidTransactionDTO
{
    public string Cursor { get; set; }
    public List<StreamlinedTransactionDTO> StreamlinedTransactionDTOs { get; set; }
}