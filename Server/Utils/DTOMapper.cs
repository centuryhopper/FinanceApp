
using System.Transactions;
using Server.Contexts;
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

    public static PlaidItemDTO ToDTO(this Plaiditem obj, EncryptionContext encryptionContext)
    {
        return new()
        {
            Plaiditemid = obj.Plaiditemid,
            Userid = obj.Userid,
            Accesstoken = encryptionContext.Decrypt(Convert.FromBase64String(obj.Accesstoken)),
            Institutionname = obj.Institutionname,
            Datelinked = obj.Datelinked,
        };
    }

    public static Plaiditem ToEntity(this PlaidItemDTO dto, EncryptionContext encryptionContext)
    {
        return new()
        {
            Plaiditemid = dto.Plaiditemid,
            Userid = dto.Userid,
            Accesstoken = Convert.ToBase64String(encryptionContext.Encrypt(dto.Accesstoken)),
            Institutionname = dto.Institutionname,
            Datelinked = dto.Datelinked,
            AccessTokenIv = dto.AccessTokenIv,
            TransactionsCursor = dto.TransactionsCursor,
        };
    }

    public static Bankinfo ToEntity(this BankInfoDTO dto)
    {
        return new()
        {
            Userid = dto.Userid,
            Bankname = dto.Bankname,
            Totalbankbalance = dto.Totalbankbalance,
            Bankinfoid = dto.Bankinfoid,
        };
    }

    public static BankInfoDTO ToDTO(this Bankinfo obj)
    {
        return new()
        {
            Userid = obj.Userid,
            Bankname = obj.Bankname,
            Totalbankbalance = obj.Totalbankbalance,
            Bankinfoid = obj.Bankinfoid,
        };
    }

    public static StreamlinedTransactionDTO ToDTO(this Streamlinedtransaction obj)
    {
        return new()
        {
            UserId = obj.Userid.Value,
            TransactionId = obj.Transactionid,
            Name = obj.Name,
            Note = obj.Note,
            Amount = obj.Amount.Value,
            Date = DateOnly.FromDateTime(obj.Date.Value),
            EnvironmentType = obj.Environmenttype,
            Category = obj.Categories.First().Name,
        };
    }

    public static Streamlinedtransaction ToEntity(this StreamlinedTransactionDTO dto)
    {
        return new()
        {
            Userid = dto.UserId,
            Transactionid = dto.TransactionId,
            Name = dto.Name,
            Note = dto.Note,
            Amount = dto.Amount,
            Date = dto.Date is null ? null : DateTime.SpecifyKind(dto.Date.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
            Environmenttype = dto.EnvironmentType,
        };
    }
}
