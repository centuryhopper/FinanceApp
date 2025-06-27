
namespace Server.Utils;

public class ConfigProvider
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public ConfigProvider(IWebHostEnvironment env, IConfiguration configuration)
    {
        _env = env;
        _configuration = configuration;
    }

    public string PlaidClientId
    {
        get => _env.IsDevelopment()
            ? _configuration["Plaid:ClientId"]
            : Environment.GetEnvironmentVariable("PlaidClientId");
    }

    public string PlaidEnvironment
    {
        get => _env.IsDevelopment()
            ? _configuration["Plaid:Env"]
            : Environment.GetEnvironmentVariable("PlaidEnv");
    }

    public string EncryptionKey
    {
        get => _env.IsDevelopment()
            ? _configuration["Encryption:Key"]
            : Environment.GetEnvironmentVariable("EncryptionKey");
    }

    public string EncryptionIV
    {
        get => _env.IsDevelopment()
            ? _configuration["Encryption:IV"]
            : Environment.GetEnvironmentVariable("EncryptionIV");
    }

    public string PlaidSandBoxSecret
    {
        get => _env.IsDevelopment()
            ? _configuration["Plaid:SandBoxSecret"]
            : Environment.GetEnvironmentVariable("PlaidSandBoxSecret");
    }

    public string PlaidProductionSecret
    {
        get => _env.IsDevelopment()
            ? _configuration["Plaid:PlaidProductionSecret"]
            : Environment.GetEnvironmentVariable("PlaidProductionSecret");
    }

    public string GetBudgetDBConnectionString
    {
        get => _env.IsDevelopment()
            ? _configuration["ConnectionStrings:BudgetDB"]
            : Environment.GetEnvironmentVariable("BudgetDB");
    }

    public string JwtKey
    {
        get => _env.IsDevelopment()
            ? _configuration["Jwt:Key"]
            : Environment.GetEnvironmentVariable("Jwt_Key");
    }

    public string JwtIssuer
    {
        get => _env.IsDevelopment()
            ? _configuration["Jwt:Issuer"]
            : Environment.GetEnvironmentVariable("Jwt_Issuer");
    }

    public string JwtAudience
    {
        get => _env.IsDevelopment()
            ? _configuration["Jwt:Audience"]
            : Environment.GetEnvironmentVariable("Jwt_Audience");
    }

    public string GetConfigurationValue(string configKey, string environmentVariableName)
    {
        return _env.IsDevelopment()
            ? _configuration[configKey]
            : Environment.GetEnvironmentVariable(environmentVariableName);
    }
}
