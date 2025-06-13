
using Server.Models;
using static Server.Models.ServiceResponses;

namespace Server.Repositories;

public interface IAccountRepository
{
    Task<GeneralResponse> Logout(int userId);
    Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
}
