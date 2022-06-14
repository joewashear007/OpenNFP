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
            FakeStorageBackend storageBackend = new();
            ChartingRepo repo = new(storageBackend);

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Open });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-5), ClearBlueResult = ClearBlueResult.Low, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-6), ClearBlueResult = ClearBlueResult.Low, Coitus = true, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-7), ClearBlueResult = ClearBlueResult.Low, MenstruationFlow = MenstruationFlow.Spotting, CervixOpening = CervixOpening.Partial });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-8), ClearBlueResult = ClearBlueResult.Unknown, MenstruationFlow = MenstruationFlow.Light, CervixOpening = CervixOpening.Partial });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-9), ClearBlueResult = ClearBlueResult.Low, MenstruationFlow = MenstruationFlow.Heavy, CervixOpening = CervixOpening.Open });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, Coitus = true, MenstruationFlow = MenstruationFlow.Spotting, CervixOpening = CervixOpening.Closed });

            var model = repo.ExportModel;

            ChartingRepo newRepo = new(storageBackend);
            await newRepo.ImportAsync(model);

            Assert.AreEqual(await repo.GetDayRecordsForCycleAsync(DateTime.Today.AddDays(-10), false).CountAsync(), await newRepo.GetDayRecordsForCycleAsync(DateTime.Today.AddDays(-10), false).CountAsync());
        }

        [TestMethod]
        public async Task ExportTask()
        {
            FakeStorageBackend storageBackend = new();
            ChartingRepo repo = new(storageBackend);
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Open });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), ClearBlueResult = ClearBlueResult.Peak, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), ClearBlueResult = ClearBlueResult.High, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-5), ClearBlueResult = ClearBlueResult.Low, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-6), ClearBlueResult = ClearBlueResult.Low, Coitus = true, CervixOpening = CervixOpening.Closed });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-7), ClearBlueResult = ClearBlueResult.Low, MenstruationFlow = MenstruationFlow.Spotting, CervixOpening = CervixOpening.Partial });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-8), ClearBlueResult = ClearBlueResult.Unknown, MenstruationFlow = MenstruationFlow.Light, CervixOpening = CervixOpening.Partial });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-9), ClearBlueResult = ClearBlueResult.Low, MenstruationFlow = MenstruationFlow.Heavy, CervixOpening = CervixOpening.Open });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, Coitus = true, MenstruationFlow = MenstruationFlow.Spotting, CervixOpening = CervixOpening.Closed });


            using MemoryStream stream = new();
            var data = repo.ExportModel;
            await JsonSerializer.SerializeAsync(stream, data);
            string streamContent = Encoding.UTF8.GetString(stream.ToArray());
            Assert.IsTrue(streamContent.Length > 1);


        }


        [TestMethod]
        public async Task ImportFileData()
        {
            FakeStorageBackend storageBackend = new();
            ChartingRepo repo = new(storageBackend);
            string data = await File.ReadAllTextAsync(@".\data\converted_data.json");
            var importData = JsonSerializer.Deserialize<ImportExportView>(data);

            await repo.ImportAsync(importData);

            Assert.IsTrue(true);


        }
    }
}
