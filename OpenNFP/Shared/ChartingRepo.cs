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
                    .Select((v, i) => new CycleIndex<Cycle>() { Index = _knownCycles.Count - i, Item = v })
                    .ToList();
            }
        }

        public ImportExportView ExportModel => new()
        {
            Cycles = _knownCycles.Values.ToList(),
            Records = _dayRepo.Values.ToList()
        };

        public int CycleCount => _knownCycles.Count;
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
            if (_knownCycles.ContainsKey(date))
            {
                _knownCycles.Remove(date);
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
                    _knownCycles[cycle.StartDate.ToKey()] = cycle;
                    _settings.UpdateStartEndDates(cycle.StartDate);
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
        }

        private async Task _computeCycleDays(bool skipLastEditDate = false)
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
            foreach(var c in secondaryData.Cycles)
            {
                _knownCycles.TryAdd(c.StartDate.ToKey(), c);
                _settings.UpdateStartEndDates(c.StartDate);
            }
            
            var changedRecords = secondaryData.Records.Where(q => q.ModifiedOn > _settings.LastSyncDate);
            foreach (var record in changedRecords)
            {
                if (await _dayRepo.ExistsAsync(record.IndexKey))
                {
                    // Both records have been modified after the sync timestamps
                    // currently taken the newest record, make this option later
                    var oldRec = await _dayRepo.GetAsync(record.IndexKey);
                    if (record.ModifiedOn > oldRec.ModifiedOn)
                    {
                        await _addUpdateRecordInternalAsync(record, false);
                    }
                }
                else
                {
                    await _addUpdateRecordInternalAsync(record, false);
                }
            }

            _settings.LastSyncDate = DateTime.UtcNow;
            await _computeCycleDays();
            await _saveSettings();
        }

        public async Task SyncAsync(IRemoteStorageBackend remoteStorage, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Sync Initiated");
            var info = await remoteStorage.GetLastSyncInfo(cancellationToken);
            if (info.SyncTimeStamp > _settings.LastSyncDate)
            {
                Logger.LogInformation("Sync Initiated");
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
            if(_knownCycles.Count > 0)
            {
                _settings.Cycles.Clear();
            }
            await _storage.WriteAsync(SETTING_KEY, _settings);
            await _storage.WriteAsync(CYCLE_KEY, _knownCycles.Values);
        }
    }
}