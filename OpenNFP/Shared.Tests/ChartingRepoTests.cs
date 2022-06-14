namespace OpenNFP.Shared.Tests
{
    [TestClass]
    public class ChartingRepoTests
    {
        [TestMethod]
        public async Task TestMethod1()
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

            Assert.AreEqual(1, repo.Cycles.Count());
            // 10 day + today since we don't add today
            Assert.AreEqual(11, await repo.GetDayRecordsForCycleAsync(DateTime.Today.AddDays(-10), false).CountAsync());
        }

        [TestMethod]
        public async Task TestAddingDates()
        {
            FakeStorageBackend storageBackend = new();
            ChartingRepo repo = new(storageBackend);

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, });

            Assert.AreEqual(1, repo.Cycles.Count());
            // 10 day + today since we don't add today
            Assert.AreEqual(11, await repo.GetDayRecordsForCycleAsync(DateTime.Today.AddDays(-10), false).CountAsync());
        }
    }
}