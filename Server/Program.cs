using Server.Services;
using Server.Models;
using Going.Plaid;
using Microsoft.OpenApi.Models;
using Server.Repositories;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.EntityFrameworkCore;
using Server.Contexts;
using Server.Utils;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<PlaidSettings>(
    builder.Configuration.GetSection("Plaid"));
builder.Services.AddPlaid(builder.Configuration.GetSection("Plaid"));

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

builder.Services.AddDbContext<FinanceAppDbContext>(options =>
{
    options.UseNpgsql(
        configProvider.GetBudgetDBConnectionString
    ).EnableSensitiveDataLogging();
});


builder.Services.AddScoped<IAccountRepository, AccountRepository>();


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

// Serve React static files
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
