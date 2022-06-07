using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace OpenNFP.Shared.Models
{
    public class ImportExportView
    {
        public List<Cycle> Cycles { get; set; }
        public List<DayRecord> Records { get; set; }
    }
}
