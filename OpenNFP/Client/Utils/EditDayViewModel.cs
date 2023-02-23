using Microsoft.Extensions.Logging;
using OpenNFP.Shared;
using OpenNFP.Shared.Interfaces;
using OpenNFP.Shared.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenNFP.Client.Utils
{
    public class EditDayViewModel
    {
        public EditDayViewModel(IChartingRepo chartingRepo)
        {
            ChartingRepo = chartingRepo;
        }

        public DateTime Date { get; set; } = DateTime.Now;
        public string DateKey { get; private set; } = DateTime.Today.ToKey();
        public int CycleDay { get; private set; } = 1;
        public int CyclePhase { get; private set; } = 1;
        public bool AdmormalTemperature { get; set; } = false;
        public decimal? Temperature { get; set; } = null;
        public bool Coitus { get; set; } = false;

        public EnumSliderHelper<ClearBlueResult> Clearblue { get; } = new EnumSliderHelper<ClearBlueResult>();
        public EnumSliderHelper<CervixOpening> CervixOpening { get; } = new EnumSliderHelper<CervixOpening>();
        public EnumSliderHelper<CervixTexture> CervixTexture { get; } = new EnumSliderHelper<CervixTexture>();
        public EnumSliderHelper<MenstruationFlow> Flow { get; } = new EnumSliderHelper<MenstruationFlow>();
        public EnumSliderHelper<TestResult> PregnancyTest { get; } = new EnumSliderHelper<TestResult>();
        public EnumSliderHelper<TestResult> OvulationTest { get; } = new EnumSliderHelper<TestResult>();
        public EnumSliderHelper<MucusSensation> MucusSensation { get; } = new EnumSliderHelper<MucusSensation>();
        public EnumSliderHelper<MucusCharacteristic> MucusCharacteristic { get; } = new EnumSliderHelper<MucusCharacteristic>();
        public EnumSliderHelper<MenstruationFlow> MenstruationFlow { get; } = new EnumSliderHelper<MenstruationFlow>();

        public string Notes { get; set; } = "";

        public bool StartCycle { get; set; } = false;

        public DateTime ModifiedOn { get; set; } = DateTime.MinValue;
        public IChartingRepo ChartingRepo { get; }

        public async Task LoadDayRecordAsync(DateTime date)
        {
            Date = date;
            DateKey = date.ToKey();
            var day = await ChartingRepo.GetDayAsync(DateKey);
            day ??= new DayRecord(DateKey);
            CycleDay = ChartingRepo.GetCycleDay(DateKey);

            Temperature = day.Temperature;
            AdmormalTemperature = day.AdmormalTemperature;
            Coitus = day.Coitus;
            Clearblue.Value = day.ClearBlueResult;
            CervixOpening.Value = day.CervixOpening;
            CervixTexture.Value = day.CervixTexture;
            Flow.Value = day.MenstruationFlow;
            PregnancyTest.Value = day.PregnancyTest;
            OvulationTest.Value = day.OvulationTest;
            MucusSensation.Value = day.MucusSensation;
            MucusCharacteristic.Value = day.MucusCharacteristic;
            MenstruationFlow.Value = day.MenstruationFlow;
            StartCycle = CycleDay == 1;
            Notes = day.Notes;
            ModifiedOn = day.ModifiedOn;
        }

        public async Task SaveDayRecord()
        {
            var day = await ChartingRepo.GetDayAsync(DateKey);
            day ??= new DayRecord(DateKey);

            day.ClearBlueResult = Clearblue.Value;
            day.CervixOpening = CervixOpening.Value;
            day.CervixTexture = CervixTexture.Value;
            day.MenstruationFlow = Flow.Value;
            day.PregnancyTest = PregnancyTest.Value;
            day.OvulationTest = OvulationTest.Value;
            day.MucusSensation = MucusSensation.Value;
            day.MucusCharacteristic = MucusCharacteristic.Value;
            day.MenstruationFlow = MenstruationFlow.Value;
            day.Notes = Notes;
            day.Temperature = Temperature;
            day.AdmormalTemperature = AdmormalTemperature;
            day.Coitus = Coitus;

            await ChartingRepo.AddUpdateRecord(day, StartCycle);
        }
    }
}
