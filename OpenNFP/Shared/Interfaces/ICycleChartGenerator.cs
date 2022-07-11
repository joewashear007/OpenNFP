using OpenNFP.Shared.Models;

namespace OpenNFP.Shared.Interfaces
{
    public interface ICycleChartGenerator
    {
        Task<CycleViewMode> GetTracesAsync(DateTime startDate, bool limitDays);
    }
}