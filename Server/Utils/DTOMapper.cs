
using System.Transactions;
using Server.Entities;
using Server.Models;
using Transaction = Server.Entities.Transaction;


namespace Server.Utils;

public static class DTOMapper
{
    public static UserDTO ToDTO(this User user, List<string> roles)
    {
        return new()
        {
            Id = user.Id,
            UmsUserid = user.UmsUserid,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Datelastlogin = user.Datelastlogin,
            Datelastlogout = user.Datelastlogout,
            Datecreated = user.Datecreated,
            Dateretired = user.Dateretired,
            Roles = roles,
        };
    }

    public static PlaidItemDTO ToDTO(this Plaiditem obj)
    {
        return new()
        {
            Plaiditemid = obj.Plaiditemid,
            Userid = obj.Userid,
            Accesstoken = obj.Accesstoken,
            Institutionname = obj.Institutionname,
            Datelinked = obj.Datelinked,
        };
    }

    public static Plaiditem ToEntity(this PlaidItemDTO dto)
    {
        return new()
        {
            Plaiditemid = dto.Plaiditemid,
            Userid = dto.Userid,
            Accesstoken = dto.Accesstoken,
            Institutionname = dto.Institutionname,
            Datelinked = dto.Datelinked,
        };
    }

    // public static ChaseTransactionsDTO ToChaseTransactionsDTO(this Transaction transaction)
    // {
    //     return new()
    //     {
    //         Transactionsid = transaction.Transactionsid,
    //         Userid = transaction.Userid,
    //         Details = transaction.Details,
    //         PostingDate = transaction.Postingdate,
    //         Description = transaction.Description,
    //         Amount = transaction.Amount.ToString(),
    //         Type = transaction.Type,
    //         Balance = transaction.Balance.ToString(),
    //         CheckOrSlip = transaction.Checkorslip.ToString(),
    //     };
    // }

    // public static Transaction ToTransactions(this ChaseTransactionsDTO dto)
    // {
    //     return new()
    //     {
    //         Transactionsid = dto.Transactionsid,
    //         Userid = dto.Userid,
    //         Details = dto.Details,
    //         Postingdate = dto.PostingDate,
    //         Description = dto.Description,
    //         Amount = string.IsNullOrWhiteSpace(dto.Amount) ? null : Convert.ToDecimal(dto.Amount),
    //         Type = dto.Type,
    //         Balance = string.IsNullOrWhiteSpace(dto.Balance) ? null : Convert.ToDecimal(dto.Balance),
    //         Checkorslip = string.IsNullOrWhiteSpace(dto.CheckOrSlip) ? null : Convert.ToInt32(dto.CheckOrSlip),
    //     };
    // }
}
