using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenNFP.Shared
{
    public class ChartingRepo : IChartingRepo
    {
        DateTime _startOfHistory;
        DateTime _firstCycleDate;
        DateTime _endOfHistory;

        private SortedDictionary<string, DayRecord> _data;
        private SortedDictionary<string, int> _cycleDayMap;
        private SortedDictionary<string, Cycle> _knownCycles;

        public IEnumerable<Cycle> Cycles { get => _knownCycles.Values; }

        public ChartingRepo()
        {
            _data = new SortedDictionary<string, DayRecord>();
            _cycleDayMap = new SortedDictionary<string, int>();
            _startOfHistory = DateTime.Today;
            _endOfHistory = DateTime.Today;
            _firstCycleDate = DateTime.Today;
            _knownCycles = new SortedDictionary<string, Cycle>();
            // Add a default empty record for today
            AddUpdateRecord(new DayRecord());
        }

        public IEnumerable<CycleDay> GetDayRecordsForCycle(DateTime cycleStart)
        {
            int i = 1;
            if (_knownCycles.TryGetValue(cycleStart.ToKey(), out Cycle? cycle))
            {
                if (cycle != null)
                {
                    DateTime day = cycleStart;
                    do
                    {
                        yield return new CycleDay { Day = _data[day.ToKey()], Index = i };
                        day = day.AddDays(1);
                        i++;
                    } while (day <= cycle.EndDate);
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                yield break;
            }
        }

        public void AddUpdateRecord(DayRecord rec, bool startNewCycle = false)
        {
            _addUpdateRecordInternal(rec, startNewCycle);

            _computeCycleDays();
        }

        private void _addUpdateRecordInternal(DayRecord rec, bool startNewCycle)
        {
            string key = rec.IndexKey;
            _data[key] = rec;
            if (rec.Date < _startOfHistory)
            {
                _startOfHistory = rec.Date;
            }
            if (rec.Date > _endOfHistory)
            {
                _endOfHistory = rec.Date;
            }
            if (startNewCycle || _knownCycles.Count == 0)
            {
                _knownCycles[key] = new Cycle
                {
                    StartDate = rec.Date,
                    Auto = true
                };
            }
            else
            {
                string firstCycleKey = _knownCycles.Keys.First();
                DateTime? firstCyle = firstCycleKey.ToDateTime();
                if (firstCyle.HasValue && rec.Date < firstCyle.Value)
                {
                    if (rec.Date.AddDays(10) > firstCyle.Value && _knownCycles[firstCycleKey].Auto)
                    {
                        _knownCycles.Remove(firstCyle.ToKey());
                    }

                    _knownCycles[key] = new Cycle
                    {
                        StartDate = rec.Date,
                        Auto = true
                    };

                }
            }
        }

        public DayRecord GetDay(string date)
        {
            return _data.GetValueOrDefault(date);
        }

        public async Task SaveAsync()
        {
            ImportExportView save = new()
            {
                Cycles = _knownCycles.Values.ToList(),
                Records = _data.Values.ToList()
            };
            using var file = new FileStream(@"C:\Temp\opennfp.json", FileMode.OpenOrCreate, FileAccess.Write);
            await JsonSerializer.SerializeAsync(file, save);
            file.Close();
        }

        public async Task OpenAsync()
        {
            using var file = new FileStream(@"C:\Temp\opennfp.json", FileMode.Open);
            var rawData = await JsonSerializer.DeserializeAsync<ImportExportView>(file);
            file.Close();
            if (rawData != null)
            {
                foreach (var cycle in rawData.Cycles)
                {
                    _knownCycles[cycle.StartDate.ToKey()] = cycle;
                }
                foreach (var rec in rawData.Records)
                {
                    _addUpdateRecordInternal(rec, false);
                }
                _computeCycleDays();
            }
        }

        private void _computeCycleDays()
        {

            if (!_knownCycles.ContainsKey(_startOfHistory.ToKey()))
            {
                _knownCycles[_startOfHistory.ToKey()] = new Cycle
                {
                    StartDate = _startOfHistory,
                    Auto = true
                };
            }

            //_findShortCycles().ToList().ForEach(q => _knownCycles.Remove(q));


            DateTime curDate = _startOfHistory;
            int cycleDay = 1;
            Cycle? prevCycle = null;
            do
            {
                string curkey = curDate.ToKey();
                if (_knownCycles.ContainsKey(curkey))
                {
                    cycleDay = 1;
                    if (prevCycle != null)
                    {
                        prevCycle.EndDate = curDate.AddDays(-1);
                    }
                    prevCycle = _knownCycles[curkey];
                }

                _cycleDayMap[curkey] = cycleDay;
                cycleDay++;
                curDate = curDate.AddDays(1);

            } while (curDate <= _endOfHistory);
            _knownCycles[_knownCycles.Keys.Last()].EndDate = _endOfHistory;
        }

        private IEnumerable<string> _findShortCycles()
        {
            int shortestCycle = 10;
            var cycleDates = _knownCycles.Keys.ToArray();
            DateTime prevCycleStart = DateTime.Parse(cycleDates[0]);
            for (int i = 1; i < cycleDates.Length; i++)
            {
                DateTime curCycleStart = DateTime.Parse(cycleDates[i]);
                if (prevCycleStart.AddDays(shortestCycle) >= curCycleStart)
                {
                    if (_knownCycles[cycleDates[i]].Auto)
                    {
                        // Let's drop cycle that are too close together
                        yield return cycleDates[i];
                    }
                    else
                    {
                        prevCycleStart = curCycleStart;
                    }
                }
                else
                {
                    prevCycleStart = curCycleStart;
                }
            }
        }
    }
}
