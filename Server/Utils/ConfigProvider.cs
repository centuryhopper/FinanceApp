
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
            : Environment.GetEnvironmentVariable("PLAID_CLIENT_ID");
    }

    public string PlaidEnvironment
    {
        get => _env.IsDevelopment()
            ? _configuration["Plaid:Env"]
            : Environment.GetEnvironmentVariable("PLAID_ENV");
    }

    public string EncryptionKey
    {
        get => _env.IsDevelopment()
            ? _configuration["Encryption:Key"]
            : Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
    }

    public string EncryptionIV
    {
        get => _env.IsDevelopment()
            ? _configuration["Encryption:IV"]
            : Environment.GetEnvironmentVariable("ENCRYPTION_IV");
    }

    public string PlaidSandBoxSecret
    {
        get => _env.IsDevelopment()
            ? _configuration["Plaid:SandBoxSecret"]
            : Environment.GetEnvironmentVariable("PLAID_SANDBOX_SECRET");
    }

    public string PlaidProductionSecret
    {
        get => _env.IsDevelopment()
            ? _configuration["Plaid:ProductionSecret"]
            : Environment.GetEnvironmentVariable("PLAID_PRODUCTION_SECRET");
    }

    public string GetBudgetDBConnectionString
    {
        get => _env.IsDevelopment()
            ? _configuration["ConnectionStrings:BudgetDB"]
            : Environment.GetEnvironmentVariable("BudgetDB_CONN");
    }

    public string JwtKey
    {
        get => _env.IsDevelopment()
            ? _configuration["Jwt:Key"]
            : Environment.GetEnvironmentVariable("JWT_SECRET");
    }

    public string JwtIssuer
    {
        get => _env.IsDevelopment()
            ? _configuration["Jwt:Issuer"]
            : Environment.GetEnvironmentVariable("JWT_ISSUER");
    }

    public string JwtAudience
    {
        get => _env.IsDevelopment()
            ? _configuration["Jwt:Audience"]
            : Environment.GetEnvironmentVariable("JWT_AUDIENCE");
    }

    public string GetConfigurationValue(string configKey, string environmentVariableName)
    {
        return _env.IsDevelopment()
            ? _configuration[configKey]
            : Environment.GetEnvironmentVariable(environmentVariableName);
    }
}
