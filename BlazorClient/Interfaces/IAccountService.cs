
using Shared.Models;
using static HandyBlazorComponents.Models.HandyServiceResponses;


namespace BlazorClient.Interfaces;

public interface IAccountService
{
    Task<HandyLoginResponse> LoginAsync(LoginDTO loginDTO);
    Task LogoutAsync();
}


