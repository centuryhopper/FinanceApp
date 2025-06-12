using Going.Plaid;
using Going.Plaid.Auth;
using Going.Plaid.Entity;
using Going.Plaid.Item;
using Going.Plaid.Link;
using Microsoft.Extensions.Options;
using Server.Models;

namespace Server.Services;

public class PlaidService
{
    private readonly PlaidClient _client;

    public PlaidService(IOptions<PlaidSettings> options)
    {
        var settings = options.Value;

        /*
        new PlaidClientOptions
        {
            ClientId = settings.ClientId,
            Secret = settings.Secret,
            Environment = settings.Environment switch
            {
                "sandbox" => PlaidEnvironment.Sandbox,
                "development" => PlaidEnvironment.Development,
                "production" => PlaidEnvironment.Production,
                _ => throw new ArgumentException("Invalid Plaid environment")
            }
        }
        */

        _client = new PlaidClient(
            settings.Environment switch
            {
                "sandbox" => Going.Plaid.Environment.Sandbox,
                "development" => Going.Plaid.Environment.Development,
                "production" => Going.Plaid.Environment.Production,
                _ => throw new ArgumentException("Invalid Plaid environment")
            },
            secret: settings.Secret,
            clientId: settings.ClientId
        );
    }

    /// <summary>
    /// gets sent up to client app
    /// </summary>
    /// <returns></returns>
    public async Task<LinkTokenCreateResponse?> CreateLinkTokenAsync(string userId)
    {
        var response = await _client.LinkTokenCreateAsync(new LinkTokenCreateRequest
        {
            ClientName = "Finance App",
            Language = Language.English,
            CountryCodes = new[] { CountryCode.Us },
            User = new LinkTokenCreateRequestUser
            {
                ClientUserId = userId
            },
            Products = new[] { Products.Auth, Products.Transactions },
            RedirectUri = null
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

        var response = await _client.ItemPublicTokenExchangeAsync(request);

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

        var response = await _client.AuthGetAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        throw new Exception("Failed to retrieve auth info: " + response.Error?.ErrorMessage);
    }


}
