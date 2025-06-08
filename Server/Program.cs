using Server.Services;
using Server.Models;
using Going.Plaid;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<PlaidSettings>(
    builder.Configuration.GetSection("Plaid"));
builder.Services.AddPlaid(builder.Configuration.GetSection("Plaid"));

builder.Services.AddHttpClient();

builder.Services.AddScoped<PlaidService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

const string CLIENT = "Allow_Clients";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        CLIENT,
        policy =>
        {
            policy
                .WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader();
                // .AllowCredentials(); // If your mobile client uses credentials like cookies or auth headers
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.MapGet("/", () => "Plaid API is running");

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(CLIENT);

app.MapControllers();

app.Run();
