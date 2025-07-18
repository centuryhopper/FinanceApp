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
using Microsoft.AspNetCore.RateLimiting;
using Server.JsonConverters;


/* 
TODO: create the streamlinedtransactions table to include these fields only DONE
TODO: Perhaps have a categories table (categoryid, category name) and a categories and transactions junction table (transactionid categoryid) DONE
TODO: create an api call that syncs the latest transactions with your application DONE
TODO: if user tries to sync a bank that is already connected then don't query any transactions and just show pop up on client side that bank has already been linked

*/


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
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<PlaidService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
}); ;
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
builder.Services.AddSingleton<EncryptionContext>();

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


// To update FinanceAppDbContext, type dotnet ef dbcontext scaffold "connection string" Npgsql.EntityFrameworkCore.PostgreSQL -o Entities -c FinanceAppDbContext --context-dir Contexts -t plaiditems -t streamlinedtransactions -t users -t category -t categorytransaction_junc -t bankinfo -t budgetcaps -f
builder.Services.AddDbContext<FinanceAppDbContext>(options =>
{
    options.UseNpgsql(
        configProvider.GetBudgetDBConnectionString
    ).EnableSensitiveDataLogging();
});

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPlaidItemRepository, PlaidItemRepository>();
builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<IStreamlinedTransactionsRepository, StreamlinedtransactionsRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();


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

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("TransactionsSyncing", options =>
    {
        // Once a day
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromHours(24);
        // options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
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


// TESTS (DONT USE IN PRODUCTION)
app.MapGet("/api/test/get-accessToken/{userId:int}/{institutionName}",
    async (int userId, string institutionName, IPlaidItemRepository plaidItemRepository) =>
{
    var record = await plaidItemRepository.GetPlaidItemAsync(userId, institutionName);

    return Results.Ok(record.First());
});

app.MapGet("/api/test/get-transactions/{accessToken}",
    async (string accessToken, PlaidService plaidService) =>
{
    var record = await plaidService.GetTransactionsAsync(accessToken);

    return Results.Ok(record);
});



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