
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog.Web.LayoutRenderers;

using LanguageExt;
using static LanguageExt.Prelude;
using Shared.Models;
using static Shared.Models.ServiceResponses;
using Server.Contexts;
using Server.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Server.Entities;


namespace Server.Repositories;

public partial class AccountRepository(
    ConfigProvider configProvider,
    HttpClient httpClient,
    FinanceAppDbContext financeAppDbContext) : IAccountRepository
{
    public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
    {
        var result =
            from dto in ValidateLoginDTO(loginDTO)
            from response in SendLoginRequest(dto)
            from loginData in ParseLoginResponse(response)
            from user in UpdateOrCreateUser(loginData)
            select GenerateToken(user.Id, user.UmsUserid, user.Email, user.Roles.First());

        return await result.Match(
            Right: token => new LoginResponse(true, token, "Login completed"),
            Left: error => new LoginResponse(false, null!, error)
        );
    }

    public async Task<GeneralResponse> Logout(int userId)
    {
        return await TryAsync(async () =>
            {
                var user = await financeAppDbContext.Users.FindAsync(userId);
                if (user is null) throw new Exception("User not found");

                user.Datelastlogout = DateTime.Now;
                await financeAppDbContext.SaveChangesAsync();
                return new GeneralResponse(true, "Log out success!");
            })
            .Match(
                Succ: res => res,
                Fail: ex => new GeneralResponse(false, ex.Message)
            );
    }

    private string GenerateToken(int userId, string userName, string email, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configProvider.JwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim("sub", userId.ToString()),
            new Claim("name", userName),
            new Claim("email", email),
            new Claim("role", role),
        };

        var token = new JwtSecurityToken(
            issuer: configProvider.JwtIssuer,
            audience: configProvider.JwtAudience,
            claims: userClaims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

public partial class AccountRepository
{
    private EitherAsync<string, LoginDTO> ValidateLoginDTO(LoginDTO loginDTO) =>
        loginDTO is not null
            ? RightAsync<string, LoginDTO>(loginDTO)
            : LeftAsync<string, LoginDTO>("Login container is empty");

    private EitherAsync<string, HttpResponseMessage> SendLoginRequest(LoginDTO loginDTO) =>
       TryAsync(async () =>
       {
           var response = await httpClient.PostAsJsonAsync("https://dotnetusermanagementsystem-production.up.railway.app/api/UMS/get-user-credentials?appName=FinanceApp", loginDTO);
           response.EnsureSuccessStatusCode();
           return response;
       })
       .ToEither(ex => ex.Message);

    private EitherAsync<string, UserDTO> UpdateOrCreateUser((string userId, string username, string email, List<string> roles) loginData) =>
        TryAsync(async () =>
        {
            var (userId, username, email, roles) = loginData;

            var existingUser = await financeAppDbContext
                .Users
                .FirstOrDefaultAsync(u => u.UmsUserid == userId);

            if (existingUser is null)
            {
                var newUser = new User
                {
                    UmsUserid = userId,
                    Email = email,
                    Datecreated = DateTime.Now,
                    Datelastlogin = DateTime.Now,
                    Dateretired = null,
                };

                await financeAppDbContext.Users.AddAsync(newUser);
                await financeAppDbContext.SaveChangesAsync();
                var newUserDTO = newUser.ToDTO(roles);
                return newUserDTO;
            }
            else
            {
                existingUser.Datelastlogin = DateTime.Now;
                await financeAppDbContext.SaveChangesAsync();
                return existingUser.ToDTO(roles);
            }
        })
        .ToEither(ex => ex.Message);

    private static EitherAsync<string, (string userId, string username, string email, List<string> roles)> ParseLoginResponse(HttpResponseMessage response) =>
        TryAsync(async () =>
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;

            var userId = root.GetProperty("userId").GetString() ?? throw new Exception("UserId missing");
            var username = root.GetProperty("username").GetString() ?? throw new Exception("Username missing");
            var email = root.GetProperty("email").GetString() ?? throw new Exception("Email missing");

            var roles = root.GetProperty("roles")
                .EnumerateArray()
                .Select(r => r.GetProperty("role").GetString() ?? "")
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .ToList();

            return (userId, username, email, roles);
        })
        .ToEither(ex => ex.Message);
}

