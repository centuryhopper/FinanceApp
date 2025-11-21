
using HandyBlazorComponents.Abstracts;
using Shared.Models;

namespace BlazorClient.Models;

public class TransactionsEntity : HandyGridEntityAbstract<StreamlinedTransactionDTO>
{
    public TransactionsEntity() : base()
    {
    }

    public TransactionsEntity(StreamlinedTransactionDTO Object) : base(Object)
    {
    }

    public override object? DisplayPropertyInGrid(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Object.Id):
                return Object.Id;
            case nameof(Object.TransactionId):
                return Object.TransactionId;
            case nameof(Object.UserId):
                return Object.UserId;
            case nameof(Object.Name):
                return Object.Name;
            case nameof(Object.Category):
                return Object.Category;
            case nameof(Object.Note):
                return Object.Note;
            case nameof(Object.Amount):
                return Object.Amount;
            case nameof(Object.Date):
                return Object.Date;
            case nameof(Object.EnvironmentType):
                return Object.EnvironmentType;
            case nameof(Object.BankInfoId):
                return Object.BankInfoId;
            default:
                System.Console.WriteLine(propertyName);
                throw new Exception("Invalid property name");
        }
    }

    public override int GetPrimaryKey()
    {
        return Object.Id;
    }

    public override void SetPrimaryKey(int id)
    {
        Object.Id = id;
    }

    public override void ParsePropertiesFromCSV(Dictionary<string, object> properties)
    {
        base.ParsePropertiesFromCSV(properties);
    }

    public override void SetProperties(Dictionary<string, object> properties)
    {
        base.SetProperties(properties);
    }
}
