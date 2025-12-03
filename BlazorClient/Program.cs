using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorClient;
using BlazorClient.Utils;
using HandyBlazorComponents.Extensions;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorClient.Providers;
using BlazorClient.Services;
using BlazorClient.Interfaces;
using BlazorClient.Handlers;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthorizationMessageHandler>();


builder.Services
.AddHttpClient(
    Constants.HTTP_CLIENT,
    client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    }
)
// need this line for being able to use httpcontext.user.claims in the server controllers
.AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(
    sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.HTTP_CLIENT)
);

builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddHandyBlazorServices();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<ThemeService>();

builder.Services.AddLucideIcons();

await builder.Build().RunAsync();

