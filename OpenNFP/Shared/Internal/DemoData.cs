using OpenNFP.Shared.Interfaces;
using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Internal
{
    public static class DemoData
    {
        public static async Task LoadDemoData(IChartingRepo Repo, DateTime date, bool clear = false)
        {
            if (clear)
            {
                Repo.Clear();
            }
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-1),
                Temperature = 99.9M,
                ClearBlueResult = ClearBlueResult.Peak,
                CervixOpening = CervixOpening.Open
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-2),
                Temperature = 99.0M,
                ClearBlueResult = ClearBlueResult.Peak,
                CervixOpening = CervixOpening.Closed
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-3),
                Temperature = 98.4M,
                ClearBlueResult = ClearBlueResult.High,
                CervixOpening = CervixOpening.Closed
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-4),
                Temperature = 98.6M,
                ClearBlueResult = ClearBlueResult.High,
                CervixOpening = CervixOpening.Closed
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-5),
                Temperature = 98.9M,
                ClearBlueResult = ClearBlueResult.Low,
                CervixOpening = CervixOpening.Closed
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-6),
                Temperature = 98.7M,
                ClearBlueResult = ClearBlueResult.Low,
                Coitus = true,
                CervixOpening = CervixOpening.Closed
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-7),
                Temperature = 98.4M,
                ClearBlueResult = ClearBlueResult.Low,
                MenstruationFlow = MenstruationFlow.Spotting,
                CervixOpening = CervixOpening.Partial
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-8),
                Temperature = 98.3M,
                ClearBlueResult = ClearBlueResult.Unknown,
                MenstruationFlow = MenstruationFlow.Light,
                CervixOpening = CervixOpening.Partial
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-9),
                Temperature = 98.2M,
                ClearBlueResult = ClearBlueResult.Low,
                MenstruationFlow = MenstruationFlow.Heavy,
                CervixOpening = CervixOpening.Open
            });
            await Repo.AddUpdateRecord(new DayRecord
            {
                Date = date.AddDays(-10),
                Temperature = 98.6M,
                ClearBlueResult = ClearBlueResult.Low,
                Coitus = true,
                MenstruationFlow = MenstruationFlow.Spotting,
                CervixOpening = CervixOpening.Closed
            });
        }
    }
}
