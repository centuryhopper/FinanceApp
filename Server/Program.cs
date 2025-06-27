using Server.Services;
using Server.Models;
using Going.Plaid;
using Microsoft.OpenApi.Models;
using Server.Repositories;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.EntityFrameworkCore;
using Server.Contexts;
using Server.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


// MUST HAVE IT LIKE THIS FOR NLOG TO RECOGNIZE DOTNET USER-SECRETS INSTEAD OF HARDCODED DELIMIT PLACEHOLDER VALUE FROM APPSETTINGS.JSON

// #if DEBUG
//     var logger = LogManager.Setup().LoadConfigurationFromFile("nlog_dev.config").GetCurrentClassLogger();
// #else
//     var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
// #endif


// try
// {


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<PlaidSettings>(
    builder.Configuration.GetSection("Plaid"));
builder.Services.AddPlaid(builder.Configuration.GetSection("Plaid"));

// Add services to the container.
// builder.Logging.ClearProviders();
// builder.Host.UseNLog();

builder.Services.AddHttpClient();

builder.Services.AddScoped<PlaidService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "oauth2",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        }
    );

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var configProvider = new ConfigProvider(builder.Environment, builder.Configuration);
builder.Services.AddSingleton(configProvider);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = configProvider.JwtIssuer,
            ValidAudience = configProvider.JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    configProvider.JwtKey
                )
            ),
        };
    });


// To update FinanceAppDbContext, type dotnet ef dbcontext scaffold "connection string" Npgsql.EntityFrameworkCore.PostgreSQL -o Entities -c FinanceAppDbContext --context-dir Contexts -t plaiditems -t transactions -t users
builder.Services.AddDbContext<FinanceAppDbContext>(options =>
{
    options.UseNpgsql(
        configProvider.GetBudgetDBConnectionString
    ).EnableSensitiveDataLogging();
});


builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPlaidItemRepository, PlaidItemRepository>();


if (!builder.Environment.IsDevelopment())
{
    var port = System.Environment.GetEnvironmentVariable("PORT") ?? "8081";
    builder.WebHost.UseUrls($"http://*:{port}");
}


const string CLIENT = "Allow_Clients";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        CLIENT,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

var app = builder.Build();

// Serve Vue static files
app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.MapGet("/", () => "Plaid API is running");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(CLIENT);

app.MapControllers();

// Catch-all route to serve index.html for React routes
app.MapFallbackToFile("index.html");

app.Run();

// }
//  catch (Exception ex)
// {
//     logger.Error(ex, "Stopped program because of exception: " + ex);
//     throw ex;
// }
// finally {
//     LogManager.Shutdown();
// }