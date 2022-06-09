using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Models
{
    public class DayRecord
    {
        private DateTime _date = DateTime.Today.Date;
        public DateTime Date
        {
            get => _date;
            set
            {
                if (value != _date)
                {
                    _date = value.Date;
                }
            }
        }

        public decimal? Temperature { get; set; }
        public ClearBlueResult ClearBlueResult { get; set; } = ClearBlueResult.Unknown;
        public bool Coitus { get; set; } = false;
        public TestResult PregancyTest { get; set; } = TestResult.Unknown;
        public TestResult OvulationTest { get; set; } = TestResult.Unknown;
        public CervixOpening CervixOpening { get; set; } = CervixOpening.Unknown;
        public CervixTexture CervixTexture { get; set; } = CervixTexture.Unknown;

        public MucusSensation MucusSensation { get; set; } = MucusSensation.Unknown;
        public MucusCharacteristic MucusCharacteristic { get; set; } = MucusCharacteristic.Unknown;

        public MenstrationFlow MenstrationFlow { get; set; } = MenstrationFlow.Unknown;
        public string Notes { get; set; } = "";

        [JsonIgnore]
        public string IndexKey => _date.ToKey();
    }
}
