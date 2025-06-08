using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Newtonsoft.Json;
using Going.Plaid.Auth;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaidController : ControllerBase
{
    private readonly PlaidService _plaid;

    public PlaidController(PlaidService plaid)
    {
        _plaid = plaid;
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

    [HttpPost("get-auth")]
    public async Task<IActionResult> GetAuthInfo([FromBody] AccessTokenRequest request)
    {
        try
        {
            var authInfo = await _plaid.GetAuthInfoAsync(request.AccessToken);
            return Ok(authInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public class AccessTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
    }


    [HttpPost("exchange-public-token")]
    public async Task<IActionResult> ExchangePublicToken([FromBody] PublicTokenRequest request)
    {
        try
        {
            var accessToken = await _plaid.ExchangePublicTokenAsync(request.PublicToken);
            return Ok(new { access_token = accessToken });
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


