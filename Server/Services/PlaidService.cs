using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Server.Models;

namespace Server.Services;

public class PlaidService
{
    private readonly HttpClient _http;
    private readonly PlaidSettings _settings;

    public PlaidService(HttpClient http, IOptions<PlaidSettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    private string BaseUrl => _settings.Environment switch
    {
        "sandbox" => "https://sandbox.plaid.com",
        "development" => "https://development.plaid.com",
        "production" => "https://production.plaid.com",
        _ => throw new Exception("Invalid Plaid environment")
    };

    public async Task<string> CreateLinkTokenAsync()
    {
        var payload = new
        {
            client_id = _settings.ClientId,
            secret = _settings.Secret,
            user = new { client_user_id = Guid.NewGuid().ToString() },
            client_name = "Plaid .NET Demo",
            products = new[] { "auth", "transactions" },
            country_codes = new[] { "US" },
            language = "en"
        };

        Console.WriteLine(payload.client_id);
        Console.WriteLine(payload.secret);

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync($"{BaseUrl}/link/token/create", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}



