using OpenNFP.Shared.Models;

namespace OpenNFP.Shared
{
    public interface IChartingRepo
    {
        IEnumerable<Cycle> Cycles { get; }

        void AddUpdateRecord(DayRecord rec, bool startNewCycle = false);
        ImportExportView Export();
        DayRecord GetDay(string date);
        IEnumerable<CycleDay> GetDayRecordsForCycle(DateTime cycleStart);
        void Import(ImportExportView rawData);
        Task OpenAsync();
        Task SaveAsync();
    }
}