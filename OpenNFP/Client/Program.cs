using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenNFP.Client;
using MudBlazor.Services;
using OpenNFP.Shared;
using OpenNFP.Shared.Models;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddSingleton<IChartingRepo, ChartingRepo>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });



var host = builder.Build();

var chartingRepo = host.Services.GetRequiredService<IChartingRepo>();

chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 99.9M, ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Open });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), Temperature = 99.0M, ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), Temperature = 98.4M, ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), Temperature = 98.6M, ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-5), Temperature = 98.9M, ClearBlueResult = ClearBlueResult.Low, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-6), Temperature = 98.7M, ClearBlueResult = ClearBlueResult.Low, Coitus = true, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-7), Temperature = 98.4M, ClearBlueResult = ClearBlueResult.Low, MenstrationFlow = MenstrationFlow.Spotting, CervixOpening = CervixOpening.Partial });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-8), Temperature = 98.3M, ClearBlueResult = ClearBlueResult.Unknown, MenstrationFlow = MenstrationFlow.Light, CervixOpening = CervixOpening.Partial });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-9), Temperature = 98.2M, ClearBlueResult = ClearBlueResult.Low, MenstrationFlow = MenstrationFlow.Heavy, CervixOpening = CervixOpening.Open });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), Temperature = 98.6M, ClearBlueResult = ClearBlueResult.Low, Coitus = true, MenstrationFlow = MenstrationFlow.Spotting, CervixOpening = CervixOpening.Closed });


await host.RunAsync();

//await builder.Build().RunAsync();
