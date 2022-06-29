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
        public DayRecord()
        {
            _date = DateTime.Today;
        }

        public DayRecord(DateTime date)
        {
            _date = date;
        }
        public DayRecord(string key)
        {
            _date = key.ToDateTime() ?? DateTime.Today;
        }

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
        public TestResult PregnancyTest { get; set; } = TestResult.Unknown;
        public TestResult OvulationTest { get; set; } = TestResult.Unknown;
        public CervixOpening CervixOpening { get; set; } = CervixOpening.Unknown;
        public CervixTexture CervixTexture { get; set; } = CervixTexture.Unknown;

        public MucusSensation MucusSensation { get; set; } = MucusSensation.Unknown;
        public MucusCharacteristic MucusCharacteristic { get; set; } = MucusCharacteristic.Unknown;

        public MenstruationFlow MenstruationFlow { get; set; } = MenstruationFlow.Unknown;
        public string Notes { get; set; } = "";

        [JsonIgnore]
        public string IndexKey => _date.ToKey();

        [JsonIgnore]
        public int MucusChartValue => Math.Max((int)MucusCharacteristic, (int)MucusSensation);

        [JsonIgnore]
        public string MucusChartLabel
        {
            get
            {
                string label = "";
                switch (MucusSensation)
                {
                    case MucusSensation.Unknown: label += " "; break;
                    case MucusSensation.None: label += "d"; break;
                    case MucusSensation.Most: label += "m"; break;
                    case MucusSensation.Watery: label += "w"; break;
                    case MucusSensation.Slippery: label += "sl "; break;
                }
                label += "/";
                switch (MucusCharacteristic)
                {
                    case MucusCharacteristic.Unknown: label += " "; break;
                    case MucusCharacteristic.None: label += "n"; break;
                    case MucusCharacteristic.Tacky: label += "t"; break;
                    case MucusCharacteristic.Strechy: label += "s"; break;
                }
                return label;
            }
        }

        [JsonIgnore]
        public int CervixChartValue => Math.Max((int)CervixOpening, (int)CervixTexture);

        [JsonIgnore]
        public string CervixChartLabel
        {
            get
            {
                string label = "";
                switch (CervixOpening)
                {
                    case CervixOpening.Unknown: label += " "; break;
                    case CervixOpening.Closed: label += "."; break;
                    case CervixOpening.Partial: label += "o"; break;
                    case CervixOpening.Open: label += "O"; break;
                }
                label += "/";
                switch (CervixTexture)
                {
                    case CervixTexture.Unknown: label += " "; break;
                    case CervixTexture.Firm: label += "f"; break;
                    case CervixTexture.Soft: label += "s"; break;
                }
                return label;
            }
        }

        [JsonIgnore]
        public int MenstruationChartValue => (int)MenstruationFlow;

        [JsonIgnore]
        public string MenstruationChartLabel
        {
            get
            {
                string label = "";
                switch (MenstruationFlow)
                {
                    case MenstruationFlow.Unknown: label += " "; break;
                    case MenstruationFlow.Spotting: label += "."; break;
                    case MenstruationFlow.Light: label += "/"; break;
                    case MenstruationFlow.Heavy: label += "X"; break;
                }
                return label;
            }
        }
    }
}
