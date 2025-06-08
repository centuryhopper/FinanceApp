using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Newtonsoft.Json;

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

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        return Ok("hello there");
    }

    [HttpGet("link-token")]
    public async Task<IActionResult> GetLinkToken()
    {
        var token = await _plaid.CreateLinkTokenAsync();
        return Ok(JsonConvert.DeserializeObject(token));
    }
}


