using OpenNFP.Shared.Models;

namespace OpenNFP.Shared
{
    public interface IChartingRepo
    {
        IEnumerable<CycleIndex<Cycle>> Cycles { get; }

        void AddUpdateRecord(DayRecord rec, bool startNewCycle = false);
        ImportExportView Export();
        int GetCycleDay(string date);
        DayRecord GetDay(string date);
        IEnumerable<CycleIndex<DayRecord>> GetDayRecordsForCycle(DateTime cycleStart);
        void Import(ImportExportView rawData);
        Task OpenAsync();
        Task SaveAsync();
        void Initialize(ChartSettings settings);

        void Clear();
        ChartSettings GetSettings();
        bool IsCycleStart(string date);
        bool DeleteCycle(string date);
    }
}