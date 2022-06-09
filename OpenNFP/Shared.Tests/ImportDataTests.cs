using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Tests
{
    [TestClass]
    public class ImportDataTests
    {
        [TestMethod]
        public async Task ImportDaysTest()
        {
            ChartingRepo repo = new ChartingRepo();

            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Open });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-5), ClearBlueResult = ClearBlueResult.Low, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-6), ClearBlueResult = ClearBlueResult.Low, Coitus = true, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-7), ClearBlueResult = ClearBlueResult.Low, MenstrationFlow = MenstrationFlow.Spotting, CervixOpening = CervixOpening.Partial });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-8), ClearBlueResult = ClearBlueResult.Unknown, MenstrationFlow = MenstrationFlow.Light, CervixOpening = CervixOpening.Partial });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-9), ClearBlueResult = ClearBlueResult.Low, MenstrationFlow = MenstrationFlow.Heavy, CervixOpening = CervixOpening.Open });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, Coitus = true, MenstrationFlow = MenstrationFlow.Spotting, CervixOpening = CervixOpening.Closed });

            await repo.SaveAsync();

            ChartingRepo newRepo = new ChartingRepo();
            await newRepo.OpenAsync();

            Assert.AreEqual(repo.GetDayRecordsForCycle(DateTime.Today.AddDays(-10)).Count(), newRepo.GetDayRecordsForCycle(DateTime.Today.AddDays(-10)).Count());
        }

        [TestMethod]
        public async Task ExportTask()
        {
            ChartingRepo repo = new ChartingRepo();
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Open });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-5), ClearBlueResult = ClearBlueResult.Low, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-6), ClearBlueResult = ClearBlueResult.Low, Coitus = true, CervixOpening = CervixOpening.Closed });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-7), ClearBlueResult = ClearBlueResult.Low, MenstrationFlow = MenstrationFlow.Spotting, CervixOpening = CervixOpening.Partial });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-8), ClearBlueResult = ClearBlueResult.Unknown, MenstrationFlow = MenstrationFlow.Light, CervixOpening = CervixOpening.Partial });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-9), ClearBlueResult = ClearBlueResult.Low, MenstrationFlow = MenstrationFlow.Heavy, CervixOpening = CervixOpening.Open });
            repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, Coitus = true, MenstrationFlow = MenstrationFlow.Spotting, CervixOpening = CervixOpening.Closed });


            using MemoryStream stream = new();
            var data = repo.Export();
            await JsonSerializer.SerializeAsync(stream, data);
            string streamContent = Encoding.UTF8.GetString(stream.ToArray());
            Assert.IsTrue(streamContent.Length > 1);


        }
    }
}
