using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Models
{
    public class ChartSettings
    {
        public static readonly Version ModelVersion = new(0, 1, 0);

        public Version Version { get; set; } = new Version(ModelVersion.ToString());
        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<Cycle> Cycles { get; set; } = new List<Cycle>();

        public int CycleDisplayLimit { get; set; } = 40;

        public int MergeAutoCycleLimit { get; set; } = 10;

        internal void UpdateStartEndDates(DateTime date)
        {
            if (date < StartDate)
            {
                StartDate = date;
            }
            if (date > EndDate)
            {
                EndDate = date;
            }
        }
    }
}
