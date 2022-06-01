using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared
{
    public class DayViewModel
    {
        public DateTime? Date { get; set; } = DateTime.Today;
        public decimal? Temperature { get; set; } 
        public int ClearBlueTest { get; set; } = 0;
        public bool Coitus { get; set; } = false;
        public string PregancyTest { get; set; } = "None";
        public string OvulationTest { get; set; } = "None";
        public int CervixFirmTest { get; set; } = 0;
        public int CervixOpenTest { get; set; } = 0;

        public int BleedingAmount { get; set; } = 0;
        public string Notes { get; set; } = "";
    }
}
