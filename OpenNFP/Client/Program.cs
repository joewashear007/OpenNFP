using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenNFP.Client;
using MudBlazor.Services;
using OpenNFP.Shared;
using OpenNFP.Shared.Models;
using Blazored.LocalStorage;
using OpenNFP.Shared.Interfaces;
using OpenNFP.Shared.Backend;
using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices(config => {
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.DefaultScopes.Add("https://www.googleapis.com/auth/drive");
});


builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddSingleton<IStorageBackend, LocalStorageBackend>();
builder.Services.AddSingleton<IChartingRepo, ChartingRepo>();
builder.Services.AddSingleton<ICycleChartGenerator, CycleChartGenerator>();

builder.Services.AddTransient<IRemoteStorageBackend, GoogleStorageBackend>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Logging.SetMinimumLevel(LogLevel.Debug);

var host = builder.Build();

var chartingRepo = host.Services.GetRequiredService<IChartingRepo>();
await chartingRepo.InitializeAsync();

await host.RunAsync();
