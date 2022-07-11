using OpenNFP.Shared.Interfaces;
using OpenNFP.Shared.Models;
using Plotly.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared
{


    public class CycleChartGenerator : ICycleChartGenerator
    {
        private IChartingRepo _chartingRepo;

        public CycleChartGenerator(IChartingRepo chartingRepo)
        {
            _chartingRepo = chartingRepo;
        }

        public async Task<CycleViewMode> GetTracesAsync(DateTime startDate, bool limitDays)
        {
            CycleViewMode vm = new CycleViewMode();
            IAsyncEnumerable<CycleIndex<DayRecord>>? days = _chartingRepo.GetDayRecordsForCycleAsync(startDate, limit: limitDays);
            await foreach (CycleIndex<DayRecord>? day in days)
            {
                if (day.Item == null)
                {
                    throw new InvalidOperationException("Day is null");
                }

                vm.Days.Add(day.Item.Date.ToString("yyyy-MM-dd"));
                if (day.Item.Temperature > 90)
                {
                    vm.Temp.Add(day.Item.Temperature);
                }
                else
                {
                    vm.Temp.Add(null, "");
                }
                vm.Monitor.Add((int)(day.Item?.ClearBlueResult ?? ClearBlueResult.Unknown), (day.Item?.ClearBlueResult ?? ClearBlueResult.Unknown).ToString()[..1]);
                vm.Cervix.Add(day.Item?.CervixChartValue ?? 0, day.Item?.CervixChartLabel ?? string.Empty);
                vm.Mucus.Add(day.Item?.MucusChartValue ?? 0, day.Item?.MucusChartLabel ?? string.Empty);
                vm.Mens.Add(day.Item?.MucusChartValue ?? 0, day.Item?.MenstruationChartLabel ?? string.Empty);
                vm.Index.Add(day.Index);
            }

            return vm;
        }
    }
}
