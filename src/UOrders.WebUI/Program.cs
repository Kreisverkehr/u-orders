using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Headers;
using UOrders.WebUI;
using UOrders.WebUI.AuthProviders;
using UOrders.WebUI.Options;
using UOrders.WebUI.Services;
using UOrders.WebUI.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddLocalization();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddHttpClient<IUOrdersService, UOrdersService>((services, client) =>
{
    client.BaseAddress = new Uri($"{builder.Configuration["api:scheme"]}://{builder.Configuration["api:host"]}:{builder.Configuration["api:port"]}/api/v1/");

    using var scope = services.CreateScope();
    var localStorage = scope.ServiceProvider.GetRequiredService<ISyncLocalStorageService>();
    if (localStorage.ContainKey("authToken"))
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", localStorage.GetItem<string>("authToken"));

    if (!localStorage.ContainKey("clientId"))
        localStorage.SetItem("clientId", Guid.NewGuid());

    client.DefaultRequestHeaders.Add("X-ClientID", localStorage.GetItem<Guid>("clientId").ToString());
});

builder.Services.AddSingleton<FrameworkDataService>();
builder.Services.AddSingleton<CartService>();
builder.Services.Configure<BrandOptions>(builder.Configuration.GetSection(BrandOptions.Brand));

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons()
    ;

await builder.Build().RunAsync();
