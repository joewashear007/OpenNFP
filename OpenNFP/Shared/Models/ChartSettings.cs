using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Models
{
    public class ChartSettings
    {
        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<Cycle> Cycles { get; set; } = new List<Cycle>();
    }
}
