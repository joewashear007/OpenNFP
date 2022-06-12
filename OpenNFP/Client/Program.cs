using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenNFP.Client;
using MudBlazor.Services;
using OpenNFP.Shared;
using OpenNFP.Shared.Models;
using Blazored.LocalStorage;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddSingleton<IChartingRepo, ChartingRepo>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();



var host = builder.Build();

var localStorage = host.Services.GetRequiredService<ILocalStorageService>();
var chartingRepo = host.Services.GetRequiredService<IChartingRepo>();


ChartSettings settings = await localStorage.GetItemAsync<ChartSettings>("chart.settings");
if (settings == null)
{
    settings = new ChartSettings();
    await localStorage.SetItemAsync("chart.settings", settings);
}
chartingRepo.Initialize(settings);

DateTime indexDate = settings.StartDate;
do
{
    Console.WriteLine($"Loading {indexDate.Date.ToShortDateString()} from local storage");
    DayRecord day = await localStorage.GetItemAsync<DayRecord>(indexDate.ToKey());
    if (day != null)
    {
        chartingRepo.AddUpdateRecord(day, false);
    }
    indexDate = indexDate.AddDays(1);

} while (indexDate <= settings.EndDate);



await host.RunAsync();

//await builder.Build().RunAsync();
