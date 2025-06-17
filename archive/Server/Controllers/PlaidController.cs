using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Newtonsoft.Json;
using Going.Plaid.Auth;
using Server.Repositories;
using Server.Models;
using System.Security.Claims;
using static Server.Models.ServiceResponses;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PlaidController : ControllerBase
{
    private readonly PlaidService _plaid;
    private readonly IPlaidItemRepository plaidItemRepository;

    public PlaidController(PlaidService plaid, IPlaidItemRepository plaidItemRepository)
    {
        _plaid = plaid;
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
            var exchangedResponse = await _plaid.ExchangePublicTokenAsync(request.PublicToken);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var authInfo = await _plaid.GetAuthInfoAsync(exchangedResponse.AccessToken);
            var bankInfo = authInfo.Accounts.Select(o => new
            {
                o.Balances,
                o.Name,
                o.OfficialName,
                o.Balances.Available,
                o.Balances.Current,
            });

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

            // return Ok(new { access_token = exchangedResponse });
            return Ok(new
            {
                bankInfo,
                authInfo.Item.InstitutionName,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpGet("get-link-token")]
    public async Task<IActionResult> GetLinkToken()
    {
        var token = await _plaid.CreateLinkTokenAsync("123");

        return Ok(token);
    }
}


