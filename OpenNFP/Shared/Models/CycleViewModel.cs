using Plotly.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Models
{
    public class CycleViewMode
    {
        public List<object> Days { get; } = new List<object>();
        public ChartTraceData Monitor { get; } = new ChartTraceData();
        public ChartTraceData Cervix { get; } = new ChartTraceData();
        public ChartTraceData Mucus { get; } = new ChartTraceData();
        public ChartTraceData Mens { get; } = new ChartTraceData();
        public ChartTraceData Temp { get; } = new ChartTraceData();
        public ChartTraceData Index { get; } = new ChartTraceData();
        public ChartTraceData Coitus { get; } = new ChartTraceData();


    }
}
