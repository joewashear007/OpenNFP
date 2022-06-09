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

        [JsonIgnore]
        public int? Lenght
        {
            get
            {
                if (EndDate.HasValue)
                {
                    return (int)(EndDate.Value - StartDate).TotalDays;
                }
                else
                {
                    return null;
                }
            }
        }

        [JsonIgnore]
        public bool Auto { get; internal set; }
    }
}