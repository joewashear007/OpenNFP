using OpenNFP.Shared.Models;

namespace OpenNFP.Shared.Interfaces
{
    public interface IChartingRepo
    {
        IEnumerable<CycleIndex<Cycle>> Cycles { get; }

        ImportExportView ExportModel { get; }

        Task AddUpdateRecord(DayRecord rec, bool startNewCycle = false);

        void Clear();

        Task<bool> DeleteCycleAsync(string date);

        int GetCycleDay(string date);

        Task<DayRecord> GetDayAsync(string date);

        IAsyncEnumerable<CycleIndex<DayRecord>> GetDayRecordsForCycleAsync(DateTime cycleStart, bool limit);

        ChartSettings GetSettings();

        Task ImportAsync(ImportExportView rawData);

        Task InitializeAsync();

        bool IsCycleStart(string date);

    }
}