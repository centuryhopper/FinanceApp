using Going.Plaid;
using Going.Plaid.Auth;
using Going.Plaid.Entity;
using Going.Plaid.Item;
using Going.Plaid.Link;
using Going.Plaid.Sandbox;
using Going.Plaid.Transactions;
using Microsoft.Extensions.Options;
using Server.Models;
using Server.Utils;
using static Server.Models.ServiceResponses;

namespace Server.Services;

public class PlaidService
{
    private readonly PlaidClient plaidClient;
    private readonly ConfigProvider configProvider;

    public PlaidService(ConfigProvider configProvider)
    {
        this.configProvider = configProvider;

        plaidClient = new PlaidClient(
            environment: configProvider.PlaidEnvironment switch
            {
                "sandbox" => Going.Plaid.Environment.Sandbox,
                "production" => Going.Plaid.Environment.Production,
                _ => throw new ArgumentException("Invalid Plaid environment")
            },
            secret: configProvider.PlaidEnvironment == "production" ? configProvider.PlaidProductionSecret : configProvider.PlaidSandBoxSecret,
            clientId: configProvider.PlaidClientId
        );
    }

    public async Task<TransactionsGetResponse> GetTransactionsAsync(string accessToken)
    {
        const int maxRetries = 5;
        const int baseDelayMs = 1000; // 1 second

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            var request = new TransactionsGetRequest
            {
                AccessToken = accessToken,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2).Date),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                Options = new TransactionsGetRequestOptions
                {
                    // Count = 150,
                    DaysRequested = 730,
                },
            };

            var response = await plaidClient.TransactionsGetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            // Check for PRODUCT_NOT_READY error
            if (response.Error?.ErrorCode == "PRODUCT_NOT_READY")
            {
                if (attempt == maxRetries)
                    throw new Exception("Transactions are still not ready after max retries.");

                int delay = baseDelayMs * attempt;
                Console.WriteLine($"Attempt {attempt}: Transactions not ready, retrying in {delay}ms...");
                await Task.Delay(delay);
                continue;
            }

            // Throw for any other errors
            throw new Exception($"Plaid API error: {response.Error?.ErrorMessage}");
        }

        throw new Exception("Unexpected error in GetTransactionsAsync");
    }


    public async Task<dynamic> GetAllTransactionsSyncAsync(string accessToken)
    {
        string cursor = null;
        var allAdded = new List<Transaction>();
        var allModified = new List<Transaction>();
        var allRemoved = new List<RemovedTransaction>();

        bool hasMore;
        do
        {
            var request = new TransactionsSyncRequest
            {
                AccessToken = accessToken,
                Cursor = cursor
            };

            var response = await plaidClient.TransactionsSyncAsync(request);

            allAdded.AddRange(response.Added);
            allModified.AddRange(response.Modified);
            allRemoved.AddRange(response.Removed);

            cursor = response.NextCursor;
            hasMore = response.HasMore;
        }
        while (hasMore);

        // Optionally convert to a custom model or return raw
        return new
        {
            cursor,
            list = allAdded
            .OrderByDescending(t => t.Date)
            .Select(t => new StreamlinedTransactionDTO
            {
                TransactionId = t.TransactionId!,
                Name = t.Name!,
                Amount = t.Amount.GetValueOrDefault(),
                Date = t.Date,
                Category = t.PersonalFinanceCategory?.Primary,
            })
            .ToList(),
        };
    }


    // public async Task<TransactionsSyncResponse> GetTransactionsSyncAsync(string accessToken)
    // {
    //     var request = new TransactionsSyncRequest
    //     {
    //         AccessToken = accessToken,
    //     };

    //     var response = await plaidClient.TransactionsSyncAsync(request);

    //     return response;
    // }


    // public async Task<TransactionsGetResponse> GetTransactionsAsync(string accessToken)
    // {
    //     var request = new TransactionsGetRequest
    //     {
    //         AccessToken = accessToken,
    //         StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2).Date),
    //         EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
    //         Options = new TransactionsGetRequestOptions
    //         {
    //             // Count = 500,
    //             DaysRequested = 730,
    //         },
    //     };

    //     var response = await plaidClient.TransactionsGetAsync(request);

    //     return response;
    // }

    /// <summary>
    /// gets sent up to client app
    /// </summary>
    /// <returns></returns>
    public async Task<LinkTokenCreateResponse?> CreateLinkTokenAsync(string userId)
    {
        var response = await plaidClient.LinkTokenCreateAsync(new LinkTokenCreateRequest
        {
            ClientName = "Finance App",
            Language = Language.English,
            CountryCodes = new[] { CountryCode.Us },
            User = new LinkTokenCreateRequestUser
            {
                ClientUserId = userId
            },
            Products = new[] { Products.Auth, Products.Transactions },
            RedirectUri = null,
            Transactions = new LinkTokenTransactions
            {
                DaysRequested = 365,
            }
        });

        if (response.IsSuccessStatusCode)
        {
            // return response.LinkToken;
            return response;
        }

        throw new Exception("Failed to create link token: " + response.Error?.ErrorMessage);
    }

    public async Task<ItemPublicTokenExchangeResponse> ExchangePublicTokenAsync(string publicToken)
    {
        var request = new ItemPublicTokenExchangeRequest
        {
            PublicToken = publicToken
        };

        var response = await plaidClient.ItemPublicTokenExchangeAsync(request);

        if (response.IsSuccessStatusCode)
        {
            // Save response.AccessToken securely (e.g., to a DB or encrypted storage)
            return response;
        }

        throw new Exception("Failed to exchange public token: " + response.Error?.ErrorMessage);
    }

    public async Task<AuthGetResponse> GetAuthInfoAsync(string accessToken)
    {
        var request = new AuthGetRequest
        {
            AccessToken = accessToken
        };

        var response = await plaidClient.AuthGetAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        throw new Exception("Failed to retrieve auth info: " + response.Error?.ErrorMessage);
    }


}
