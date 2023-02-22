using OpenNFP.Shared.Interfaces;
using OpenNFP.Shared.Models;
using Plotly.Blazor;
using Plotly.Blazor.Traces.Mesh3DLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
                else
                {

                    vm.Days.Add(day.Item.Date.ToString("yyyy-MM-dd"));
                    if (day.Item.Temperature > 90)
                    {
                        vm.Temp.Add(day.Item.Temperature);
                    }
                    else
                    {
                        vm.Temp.Add(null, "");
                    }
                    vm.Monitor.Add(day.Item.ClearBlueResult.ToChartValue(), day.Item.ClearBlueResult.ToChartLabel());

                    var cervixInfo = EnumMapper.CombineCervixValue(day.Item.CervixOpening, day.Item.CervixTexture);
                    vm.Cervix.Add(cervixInfo.value, cervixInfo.label);

                    var mucusInfo = EnumMapper.CombineMucusValue(day.Item.MucusSensation, day.Item.MucusCharacteristic);
                    vm.Mucus.Add(mucusInfo.value, mucusInfo.label);

                    vm.Mens.Add(day.Item.MenstruationFlow.ToChartValue(), day.Item.MenstruationFlow.ToChartLabel());
                    vm.Coitus.Add(day.Item?.Coitus);

                    string indexStr = "";
                    if (day.Item?.Coitus ?? false)
                    {
                        indexStr += "🤍";
                    }
                    if (!string.IsNullOrEmpty(day.Item?.Notes))
                    {
                        indexStr += "📄";
                    }
                    if (indexStr != "")
                    {
                        indexStr = day.Index.ToString() + "<br />" + indexStr;
                    }
                    else
                    {
                        indexStr = day.Index.ToString();
                    }
                    vm.Index.Add(day.Index, indexStr);
                }
            }

            return vm;
        }
    }
}
