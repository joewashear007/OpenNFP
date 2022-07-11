using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Models
{
    public class Cycle
    {

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime? EndDate { get; set; } = null;
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Calculated Length, it is inclusive of each date so it might be 1 more than expected
        /// </summary>
        [JsonIgnore]
        public int? Length
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return (int)Math.Floor((EndDate.Value.Date - StartDate.Date).TotalDays) + 1;
                }
                else
                {
                    return 1;
                }
            }
        }

        [JsonIgnore]
        public bool Auto { get; internal set; }
    }
}