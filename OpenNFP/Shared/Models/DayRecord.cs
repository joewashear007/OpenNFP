using Plotly.Blazor.Traces.SankeyLib;
using System.Text.Json.Serialization;

namespace OpenNFP.Shared.Models
{
    public class DayRecord
    {
        private DateTime _date = DateTime.Today.Date;
        private decimal? _temperature;
        private ClearBlueResult _clearBlueResult = ClearBlueResult.Unknown;
        private bool _coitus = false;
        private TestResult _pregnancyTest = TestResult.Unknown;
        private TestResult _ovulationTest = TestResult.Unknown;
        private CervixOpening _cervixOpening = CervixOpening.Unknown;
        private CervixTexture _cervixTexture = CervixTexture.Unknown;
        private MucusSensation _mucusSensation = MucusSensation.Unknown;
        private MucusCharacteristic _mucusCharacteristic = MucusCharacteristic.Unknown;
        private MenstruationFlow _menstruationFlow = MenstruationFlow.Unknown;
        private string _notes = "";

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

        /// <summary>
        /// The time the record was modifed, should be in UTC time
        /// </summary>
        /// <remarks>This property should be read/write last by serilizer to properly set value</remarks>
        [JsonPropertyOrder(1000)]
        public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The Date of the Record
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set
            {
                if (value != _date)
                {
                    _date = value.Date;
                    ModifiedOn = DateTime.UtcNow;
                }
            }
        }

        public decimal? Temperature
        {
            get => _temperature;
            set { if (value != _temperature) { _temperature = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public ClearBlueResult ClearBlueResult
        {
            get => _clearBlueResult;
            set { if (value != _clearBlueResult) { _clearBlueResult = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public bool Coitus
        {
            get => _coitus;
            set { if (value != _coitus) { _coitus = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public TestResult PregnancyTest
        {
            get => _pregnancyTest;
            set { if (value != _pregnancyTest) { _pregnancyTest = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public TestResult OvulationTest
        {
            get => _ovulationTest;
            set { if (value != _ovulationTest) { _ovulationTest = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public CervixOpening CervixOpening
        {
            get => _cervixOpening;
            set { if (value != _cervixOpening) { _cervixOpening = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public CervixTexture CervixTexture
        {
            get => _cervixTexture;
            set { if (value != _cervixTexture) { _cervixTexture = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public MucusSensation MucusSensation
        {
            get => _mucusSensation;
            set { if (value != _mucusSensation) { _mucusSensation = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public MucusCharacteristic MucusCharacteristic
        {
            get => _mucusCharacteristic;
            set { if (value != _mucusCharacteristic) { _mucusCharacteristic = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public MenstruationFlow MenstruationFlow
        {
            get => _menstruationFlow;
            set { if (value != _menstruationFlow) { _menstruationFlow = value; ModifiedOn = DateTime.UtcNow; } }
        }
        public string Notes
        {
            get => _notes;
            set { if (value != _notes) { _notes = value; ModifiedOn = DateTime.UtcNow; } }
        }

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


        public bool IsEmpty()
        {
            return _temperature == null
                && _clearBlueResult == ClearBlueResult.Unknown
                && _coitus == false
                && _pregnancyTest == TestResult.Unknown
                && _ovulationTest == TestResult.Unknown
                && _cervixOpening == CervixOpening.Unknown
                && _cervixTexture == CervixTexture.Unknown
                && _mucusSensation == MucusSensation.Unknown
                && _mucusCharacteristic == MucusCharacteristic.Unknown
                && _menstruationFlow == MenstruationFlow.Unknown
                && _notes == "";
        }

        public override string ToString()
        {
            string value = $"{Date.ToKey()} - ";
            foreach (var p in typeof(DayRecord).GetProperties().Where(q => q.PropertyType != typeof(DateTime)))
            {
                value += " " + p.Name + ":" + p.GetValue(this);
            }

            return value;
        }
    }
}
