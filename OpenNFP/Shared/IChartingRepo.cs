using OpenNFP.Shared.Models;

namespace OpenNFP.Shared
{
    public interface IChartingRepo
    {
        IEnumerable<Cycle> Cycles { get; }

        void AddUpdateRecord(DayRecord rec, bool startNewCycle = false);
        DayRecord GetDay(string date);
        IEnumerable<DayRecord> GetDayRecordsForCycle(DateTime cycleStart);
        Task OpenAsync();
        Task SaveAsync();
    }
}