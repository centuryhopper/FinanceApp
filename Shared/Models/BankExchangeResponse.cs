
namespace Shared.Models;

public class BankExchangeResponse
{
    public string? Status { get; set; }
    public string? ErrorMessage { get; set; }
    public BankInfoDTO? BankInfo { get; set; }
}
