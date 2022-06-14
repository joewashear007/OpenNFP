using OpenNFP.Shared.Internal;
using OpenNFP.Shared.Models;
using System.Text.Json;

namespace OpenNFP.Shared
{
    public class ChartingRepo : IChartingRepo
    {
        public const string SETTING_KEY = "chart.settings";

        private readonly IStorageBackend _storage;
        private ChartSettings _settings;
        private readonly DayRepo _dayRepo;
        private readonly SortedDictionary<string, int> _cycleDayMap;
        private readonly SortedDictionary<string, Cycle> _knownCycles;

        public IEnumerable<CycleIndex<Cycle>> Cycles
        {
            get
            {
                return _knownCycles.Values
                    .Select((v, i) => new CycleIndex<Cycle>() { Index = i + 1, Item = v })
                    .ToList();
            }
        }

        public ImportExportView ExportModel => new()
        {
            Cycles = _knownCycles.Values.ToList(),
            Records = _dayRepo.Values.ToList()
        };


        public ChartingRepo(IStorageBackend storage)
        {
            _storage = storage;
            _dayRepo = new DayRepo(storage);
            _settings = new ChartSettings();
            _cycleDayMap = new SortedDictionary<string, int>();
            _knownCycles = new SortedDictionary<string, Cycle>();
        }

        public async IAsyncEnumerable<CycleIndex<DayRecord>> GetDayRecordsForCycle(DateTime cycleStart, bool limit)
        {
            int i = 1;
            if (_knownCycles.TryGetValue(cycleStart.ToKey(), out Cycle? cycle))
            {
                if (cycle != null)
                {
                    DateTime day = cycleStart;
                    bool displayLimitReached = false;

                    do
                    {
                        displayLimitReached = i >= _settings.CycleDisplayLimit && limit;
                        yield return new CycleIndex<DayRecord> { Item = await _dayRepo.GetAsync(day), Index = i };
                        day = day.AddDays(1);
                        i++;
                    } while (day <= cycle.EndDate && !displayLimitReached);
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

        public async Task AddUpdateRecord(DayRecord rec, bool startNewCycle = false)
        {
            await _addUpdateRecordInternalAsync(rec, startNewCycle);
            await _computeCycleDays();
        }



        private async Task _addUpdateRecordInternalAsync(DayRecord rec, bool startNewCycle)
        {
            string key = rec.IndexKey;
            await _dayRepo.SetAsync(rec);
            _settings.UpdateStartEndDates(rec.Date);

            if (startNewCycle || _knownCycles.Count == 0)
            {
                _addAutoCycle(rec.Date);
            }
            else
            {
                // Combine Cycle if next cycle is with 10 days and is an auto cycle
                string firstCycleKey = _knownCycles.Keys.First();
                DateTime? firstCyle = firstCycleKey.ToDateTime();
                if (firstCyle.HasValue && rec.Date < firstCyle.Value)
                {
                    if (rec.Date.AddDays(10) > firstCyle.Value && _knownCycles[firstCycleKey].Auto)
                    {
                        _knownCycles.Remove(firstCyle.ToKey());
                    }
                    _addAutoCycle(rec.Date);
                }
            }
        }

        public async Task<DayRecord> GetDayAsync(string date)
        {
            return await _dayRepo.GetAsync(date);
        }

        public bool IsCycleStart(string date)
        {
            return _knownCycles.ContainsKey(date);
        }

        public async Task<bool> DeleteCycleAsync(string date)
        {
            if (_knownCycles.ContainsKey(date))
            {
                _knownCycles.Remove(date);
                await _storage.WriteAsync(SETTING_KEY, GetSettings());
                return true;
            }
            return false;
        }

        public int GetCycleDay(string date)
        {
            if (_cycleDayMap.TryGetValue(date, out int day))
            {
                return day;
            }
            else
            {
                return -1;
            }
        }



        public async Task SaveAsync()
        {
            ImportExportView save = new()
            {
                Cycles = _knownCycles.Values.ToList(),
                Records = _dayRepo.Values.ToList()
            };
            using var file = new FileStream(@"C:\Temp\opennfp.json", FileMode.OpenOrCreate, FileAccess.Write);
            await JsonSerializer.SerializeAsync(file, save);
            file.Close();
        }

        public async Task ImportAsync(ImportExportView rawData)
        {
            if (rawData != null)
            {
                foreach (var cycle in rawData.Cycles)
                {
                    _knownCycles[cycle.StartDate.ToKey()] = cycle;
                    _settings.UpdateStartEndDates(cycle.StartDate);
                }
                foreach (var rec in rawData.Records)
                {
                    await _addUpdateRecordInternalAsync(rec, false);
                }
                await _computeCycleDays();
            }
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
                    await _addUpdateRecordInternalAsync(rec, false);
                }
                await _computeCycleDays();
            }
        }

        private void _addAutoCycle(DateTime date)
        {
            string key = date.ToKey();
            if (!_knownCycles.ContainsKey(key))
            {
                _knownCycles[key] = new Cycle() { Auto = true, StartDate = date };
            }
        }

        private async Task _computeCycleDays()
        {
            _addAutoCycle(_settings.StartDate);

            DateTime curDate = _settings.StartDate;
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
                if (!await _dayRepo.ExistsAsync(curkey))
                {
                    await _dayRepo.SetAsync(new DayRecord(curDate));
                }

                _cycleDayMap[curkey] = cycleDay;
                cycleDay++;
                curDate = curDate.AddDays(1);
            } while (curDate <= _settings.EndDate);
            _knownCycles[_knownCycles.Keys.Last()].EndDate = _settings.EndDate;
            await _storage.WriteAsync(SETTING_KEY, GetSettings());
        }


        public async Task InitializeAsync()
        {

            var loadedSettings = await _storage.ReadAsync<ChartSettings>(SETTING_KEY);
            if (loadedSettings != null)
            {
                _settings = loadedSettings;
            }
            else
            {
                await _storage.WriteAsync(SETTING_KEY, _settings);
            }


        }

        public ChartSettings GetSettings()
        {
            return new ChartSettings()
            {
                StartDate = _settings.StartDate,
                EndDate = _settings.EndDate,
                Cycles = _knownCycles.Values.ToList()
            };
        }

        public void Clear()
        {
            _settings = new ChartSettings();
            _knownCycles.Clear();
            _cycleDayMap.Clear();
            _dayRepo.Clear();
        }
    }
}