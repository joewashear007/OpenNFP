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

chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Open });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-5), ClearBlueResult = ClearBlueResult.Low, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-6), ClearBlueResult = ClearBlueResult.Low, Coitus = true, CervixOpening = CervixOpening.Closed });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-7), ClearBlueResult = ClearBlueResult.Low, BleedingAmount = Flow.Spotting, CervixOpening = CervixOpening.Partial });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-8), ClearBlueResult = ClearBlueResult.Unknown, BleedingAmount = Flow.Light, CervixOpening = CervixOpening.Partial });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-9), ClearBlueResult = ClearBlueResult.Low, BleedingAmount = Flow.Heavy, CervixOpening = CervixOpening.Open });
chartingRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, Coitus = true, BleedingAmount = Flow.Spotting, CervixOpening = CervixOpening.Closed });


await host.RunAsync();

//await builder.Build().RunAsync();
