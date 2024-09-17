using Microsoft.Extensions.Logging;
using OpenNFP.Shared.Interfaces;
using OpenNFP.Shared.Internal;
using OpenNFP.Shared.Models;
using System.Text.Json;
using System.Threading;

namespace OpenNFP.Shared
{
    public class ChartingRepo : IChartingRepo
    {
        public const string SETTING_KEY = "chart.settings";
        public const string CYCLE_KEY = "chart.cycles";

        private readonly IStorageBackend _storage;
        private ChartSettings _settings;
        private readonly DayRepo _dayRepo;
        private readonly SortedDictionary<string, int> _cycleDayMap;
        private readonly SortedDictionary<string, Cycle> _knownCycles;

        public IEnumerable<CycleIndex<Cycle>> Cycles
        {
            get
            {
                return _knownCycles.Values.Reverse()
                    .Where(q => !q.Deleted)
                    .Select((v, i) => new CycleIndex<Cycle>() { Index = _knownCycles.Count - i, Item = v })
                    .ToList();
            }
        }

        public IEnumerable<CycleIndex<Cycle>> GetCycles(bool skipDeleted = true, bool sortNewestFirst = true)
        {
            var _searchCycles = _knownCycles.Values.AsEnumerable<Cycle>();
            if (sortNewestFirst)
            {
                _searchCycles = _searchCycles.Reverse();
            }
            if (skipDeleted)
            {
                _searchCycles = _searchCycles.Where(q => !q.Deleted);
            }

            return _searchCycles
                .Select((v, i) => new CycleIndex<Cycle>() { Index = _knownCycles.Count - i, Item = v })
                .ToList();
        }

        public ImportExportView ExportModel => new()
        {
            Cycles = _knownCycles.Values.Where(q => !q.Deleted).ToList(),
            Records = _dayRepo.Values.ToList()
        };

        public int CycleCount => _knownCycles.Values.Where(q => !q.Deleted).Count();
        public ChartSettings Settings => _settings;

        private ILogger<IChartingRepo> Logger { get; }

        public ChartingRepo(IStorageBackend storage, ILogger<IChartingRepo> logger)
        {
            _storage = storage;
            Logger = logger;
            _dayRepo = new DayRepo(storage);
            _settings = new ChartSettings();
            _cycleDayMap = new SortedDictionary<string, int>();
            _knownCycles = new SortedDictionary<string, Cycle>();
        }

        public async IAsyncEnumerable<CycleIndex<DayRecord>> GetDayRecordsForCycleAsync(DateTime cycleStart, bool limit)
        {
            int i = 1;
            if (_knownCycles.TryGetValue(cycleStart.ToKey(), out Cycle? cycle))
            {
                if (cycle != null)
                {
                    DateTime day = cycleStart;
                    bool displayLimitReached;
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
            await _saveSettings();
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
            if (_knownCycles.TryGetValue(date, out Cycle value))
            {
                string? prevKey = _knownCycles.Keys.TakeWhile(q => q != date).LastOrDefault(q => !_knownCycles[q].Deleted);
                if (prevKey != null && _knownCycles.TryGetValue(prevKey, out Cycle prevCyle))
                {
                    prevCyle.EndDate = value.EndDate;
                }
                value.Deleted = true;
                await _saveSettings();
                return true;
            }
            return false;
        }

        public async Task<bool> RestoreCycleAsync(string date)
        {
            if (_knownCycles.TryGetValue(date, out Cycle value))
            {
                value.Deleted = false;
                await _computeCycleDays();
                await _saveSettings();
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


        public async Task ImportAsync(ImportExportView rawData)
        {
            if (rawData != null)
            {
                foreach (var cycle in rawData.Cycles)
                {
                    if (!cycle.Deleted)
                    {
                        _knownCycles[cycle.StartDate.ToKey()] = cycle;
                        _settings.UpdateStartEndDates(cycle.StartDate);
                    }
                }
                foreach (var rec in rawData.Records)
                {
                    await _addUpdateRecordInternalAsync(rec, false);
                }
                await _computeCycleDays();
                await _saveSettings();
            }
        }



        private void _addAutoCycle(DateTime date)
        {
            string key = date.ToKey();
            if (!_knownCycles.ContainsKey(key))
            {
                _knownCycles[key] = new Cycle() { Auto = true, StartDate = date };
            }
            else
            {
                _knownCycles[key].Deleted = false;
            }
        }

        private async Task _computeCycleDays(bool skipLastEditDate = false)
        {
            _addAutoCycle(_settings.StartDate);

            if (_knownCycles.Count > 2)
            {
                // find and auto delete short cycles
                var cycleStartDays = _knownCycles.Where(q => !q.Value.Deleted).Select(q => q.Key).ToList();

                for (int i = 2; i < cycleStartDays.Count; i++)
                {
                    var prevCycleStart = cycleStartDays[i - 1];
                    var curCycleStart = cycleStartDays[i];
                    var daySpan = Convert.ToInt32(curCycleStart) - Convert.ToInt32(prevCycleStart);
                    if (daySpan < 5)
                    {
                        _knownCycles[prevCycleStart].Deleted = true;
                    }

                }
            }


            DateTime curDate = _settings.StartDate;
            int cycleDay = 1;
            Cycle? prevCycle = null;
            do
            {
                string curkey = curDate.ToKey();
                if (_knownCycles.TryGetValue(curkey, out Cycle? value))
                {
                    if (value != null && !value.Deleted)
                    {
                        cycleDay = 1;
                        if (prevCycle != null)
                        {
                            prevCycle.EndDate = curDate.AddDays(-1);
                        }
                        prevCycle = _knownCycles[curkey];
                    }
                }
                if (!await _dayRepo.ExistsAsync(curkey))
                {
                    await _dayRepo.SetAsync(new DayRecord(curDate));
                }

                _cycleDayMap[curkey] = cycleDay;
                cycleDay++;
                curDate = curDate.AddDays(1);
            } while (curDate <= _settings.EndDate);

            string lastValidCycle = _knownCycles.Last(q => !q.Value.Deleted).Key;
            _knownCycles[lastValidCycle].EndDate = _settings.EndDate;
        }


        public async Task InitializeAsync()
        {
            var loadedCycles = await _storage.ReadAsync<List<Cycle>>(CYCLE_KEY);
            loadedCycles?.ForEach(q => _knownCycles.TryAdd(q.StartDate.ToKey(), q));

            var loadedSettings = await _storage.ReadAsync<ChartSettings>(SETTING_KEY);
            if (loadedSettings != null)
            {
                _settings = loadedSettings;
                if (loadedSettings.EndDate < DateTime.Today)
                {
                    //TODO: should this be a app settings?
                    _settings.EndDate = DateTime.Today;
                }
                DateTime? cycleStart = _knownCycles.Keys.First().ToDateTime();
                if (cycleStart.HasValue && loadedSettings.StartDate > cycleStart.Value)
                {
                    //TODO: should this be a app settings?
                    _settings.StartDate = cycleStart.Value;
                }

                // TODO: remove, this is an old beahavior
                loadedSettings.Cycles.ForEach(q => _knownCycles.TryAdd(q.StartDate.ToKey(), q));
            }
            else
            {
                await _saveSettings();
            }
            await _computeCycleDays();
        }



        public void Clear()
        {
            _settings = new ChartSettings();
            _knownCycles.Clear();
            _cycleDayMap.Clear();
            _dayRepo.Clear();
        }

        public async Task MergeAsync(ImportExportView secondaryData)
        {
            foreach (var c in secondaryData.Cycles)
            {
                if (!c.Deleted)
                {
                    _knownCycles.TryAdd(c.StartDate.ToKey(), c);
                    _settings.UpdateStartEndDates(c.StartDate);
                }
            }

            var changedRecords = secondaryData.Records.Where(q => q.ModifiedOn > _settings.LastSyncDate && !q.IsEmpty());
            foreach (var incommingRecord in changedRecords)
            {
                if (await _dayRepo.ExistsAsync(incommingRecord.IndexKey))
                {
                    var currentRecord = await _dayRepo.GetAsync(incommingRecord.IndexKey);

                    // If the current record is empty, take the new one
                    if (currentRecord.IsEmpty())
                    {
                        await _addUpdateRecordInternalAsync(incommingRecord, false);
                    }

                    // Both records have been modified after the sync timestamps
                    // currently taken the newest record, make this option later
                    if (incommingRecord.ModifiedOn > currentRecord.ModifiedOn)
                    {
                        await _addUpdateRecordInternalAsync(incommingRecord, false);
                    }
                }
                else
                {
                    await _addUpdateRecordInternalAsync(incommingRecord, false);
                }
            }

            _settings.LastSyncDate = DateTime.UtcNow;
            await _computeCycleDays();
            await _saveSettings();
        }

        public async Task SyncAsync(IRemoteStorageBackEnd remoteStorage, string filename, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Getting Sync Info, {filename}", filename);
            var info = await remoteStorage.GetLastSyncInfo(filename, cancellationToken);
            if (info.SyncTimeStamp > _settings.LastSyncDate)
            {
                Logger.LogInformation("Sync Initiated, {filename}", filename);
                var data = await remoteStorage.ReadAsync<ImportExportView>(info, cancellationToken);
                if (data is not null)
                {
                    await MergeAsync(data);
                    await remoteStorage.WriteAsync(info, ExportModel, cancellationToken);
                }
            }
            else
            {
                Logger.LogInformation("Sync Skipped Due to TimeStamps, {LastSync} >= {RemoteUpdate}", _settings.LastSyncDate, info.SyncTimeStamp);
                if (info.SyncTimeStamp < _settings.LastEditDate)
                {
                    Logger.LogInformation("Sync Initiated - Local Edits");
                    var data = await remoteStorage.ReadAsync<ImportExportView>(info, cancellationToken);
                    if (data is not null)
                    {
                        await MergeAsync(data);
                        await remoteStorage.WriteAsync(info, ExportModel, cancellationToken);
                    }
                }
                else
                {
                    Logger.LogInformation("Sync Skipped Due to TimeStamps, {LastEdit} >= {RemoteUpdate}", _settings.LastEditDate, info.SyncTimeStamp);
                }
            }
            Logger.LogInformation("Sync Complete");
        }

        private async Task _saveSettings(bool skipLastEditDate = false)
        {
            if (!skipLastEditDate)
            {
                _settings.LastEditDate = DateTime.UtcNow;
            }
            if (_knownCycles.Count > 0)
            {
                _settings.Cycles.Clear();
            }
            await _storage.WriteAsync(SETTING_KEY, _settings);
            await _storage.WriteAsync(CYCLE_KEY, _knownCycles.Values);
        }
    }
}