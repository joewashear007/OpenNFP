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
        private DateTime startDate = DateTime.Today;
        private DateTime? endDate = null;
        private string notes = string.Empty;

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if (value != startDate)
                {
                    startDate = value.Date;
                    ModifiedOn = DateTime.UtcNow;
                }
            }
        }
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                if (value != endDate)
                {
                    endDate = value;
                    ModifiedOn = DateTime.UtcNow;
                }
            }
        }
        public string Notes
        {
            get => notes;
            set
            {
                if (value != notes )
                {
                    notes = value;
                    ModifiedOn = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// The time the record was modifed, should be in UTC time
        /// </summary>
        /// <remarks>This property should be read/write last by serilizer to properly set value</remarks>
        [JsonPropertyOrder(1000)]
        public DateTime ModifiedOn { get; set; } = DateTime.MinValue;

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