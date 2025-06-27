using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Newtonsoft.Json;
using Going.Plaid.Auth;
using Server.Repositories;
using Server.Models;
using System.Security.Claims;
using static Server.Models.ServiceResponses;
using Microsoft.AspNetCore.Authorization;
using Server.Utils;
using System.Text;
using System.Text.Json;
using Going.Plaid.Transactions;

namespace Server.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PlaidController : ControllerBase
{
    private readonly PlaidService plaidService;
    private readonly IPlaidItemRepository plaidItemRepository;
    private readonly ConfigProvider configProvider;

    public PlaidController(PlaidService plaidService, IPlaidItemRepository plaidItemRepository, ConfigProvider configProvider)
    {
        this.configProvider = configProvider;
        this.plaidService = plaidService;
        this.plaidItemRepository = plaidItemRepository;
    }

    public class PublicTokenRequest
    {
        public string PublicToken { get; set; } = string.Empty;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        return Ok("hello there");
    }

    public class AccessTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
    }

    [Authorize]
    [HttpPost("exchange-public-token")]
    public async Task<IActionResult> ExchangePublicToken([FromBody] PublicTokenRequest request)
    {
        try
        {
            var exchangedResponse = await plaidService.ExchangePublicTokenAsync(request.PublicToken);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var authInfo = await plaidService.GetAuthInfoAsync(exchangedResponse.AccessToken);
            var bankInfo = authInfo.Accounts.Select(o => new
            {
                o.Balances,
                o.Name,
                o.OfficialName,
                o.Balances.Available,
                o.Balances.Current,
            });

            TransactionsGetResponse? transactionResponse = await plaidService.GetTransactionsAsync(exchangedResponse.AccessToken);

            var result = await plaidItemRepository.GetPlaidItemAsync(userId, authInfo.Item.InstitutionName).Match(
                Some: _ => { },
                None: async () =>
                {
                    // store access token in db instead of returning it to the client!
                    var response = (await plaidItemRepository.StorePlaidItemAsync(new PlaidItemDTO
                    {
                        Userid = userId,
                        Accesstoken = exchangedResponse.AccessToken,
                        Institutionname = authInfo.Item.InstitutionName,
                        Datelinked = DateTime.UtcNow,
                    })).Match(
                        Left: msg => new GeneralResponse(false, msg),
                        Right: _ => _
                    );
                }
            );



            return Ok(new
            {
                transactionResponse,
                bankInfo,
                name = authInfo.Item.InstitutionName,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [Authorize]
    [HttpGet("get-link-token/{userId}")]
    public async Task<IActionResult> GetLinkToken(string userId)
    {
        var linkToken = await plaidService.CreateLinkTokenAsync(userId);
        return Ok(linkToken);
    }

    [HttpPost("create-sandbox-token")]
    public async Task<IActionResult> CreateSandboxPublicToken()
    {
        using var httpClient = new HttpClient();
        var payload = new
        {
            client_id = configProvider.PlaidClientId,
            secret = configProvider.PlaidSandBoxSecret,
            institution_id = "ins_109508",
            initial_products = new[] { "transactions" },
            options = new
            {
                transactions = new
                {
                    days_requested = 730
                }
            }
        };

        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://sandbox.plaid.com/sandbox/public_token/create", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, responseContent);
        }

        var json = JsonDocument.Parse(responseContent);
        var publicToken = json.RootElement.GetProperty("public_token").GetString();

        return Ok(new { public_token = publicToken });
    }

}


