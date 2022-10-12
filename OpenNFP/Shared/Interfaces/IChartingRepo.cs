using OpenNFP.Shared.Models;

namespace OpenNFP.Shared.Interfaces
{
    public interface IChartingRepo
    {
        int CycleCount { get; }
        IEnumerable<CycleIndex<Cycle>> Cycles { get; }

        ImportExportView ExportModel { get; }

        Task AddUpdateRecord(DayRecord rec, bool startNewCycle = false);

        void Clear();

        Task<bool> DeleteCycleAsync(string date);

        int GetCycleDay(string date);

        Task<DayRecord> GetDayAsync(string date);

        IAsyncEnumerable<CycleIndex<DayRecord>> GetDayRecordsForCycleAsync(DateTime cycleStart, bool limit);

        ChartSettings Settings { get; }

        Task ImportAsync(ImportExportView rawData);

        Task InitializeAsync();

        bool IsCycleStart(string date);

        Task MergeAsync(ImportExportView secondaryData);
        Task SyncAsync(IRemoteStorageBackend remoteStorage, CancellationToken cancellationToken);
    }
}