namespace OpenNFP.Shared.Tests
{
    [TestClass]
    public class ChartingRepoTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            FakeStorageBackend storageBackend = new();
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());

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
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, });

            Assert.AreEqual(1, repo.Cycles.Count());
            // 10 day + today since we don't add today
            Assert.AreEqual(11, await repo.GetDayRecordsForCycleAsync(DateTime.Today.AddDays(-10), false).CountAsync());
        }

        [TestMethod]
        public async Task Sync_AllDiffernceDays()
        {
            ChartingRepo repo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            var syncTime = repo.Settings.LastSyncDate;

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), });


            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), });

            await repo.MergeAsync(secondaryRepo.ExportModel);

            Assert.AreEqual(2, repo.Cycles.Count());
            Assert.AreEqual(5, repo.ExportModel.Records.Count);
            Assert.IsTrue(repo.Settings.LastSyncDate > syncTime, $"Sync Time {repo.Settings.LastSyncDate} should be greater than {syncTime}");
        }

        [TestMethod]
        public async Task Sync_SameDaysSecondaryNewer()
        {
            ChartingRepo repo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            var syncTime = repo.Settings.LastSyncDate;

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 98.6M, ModifiedOn = DateTime.UtcNow.AddHours(-2) });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), });


            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 99.0M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), });

            await repo.MergeAsync(secondaryRepo.ExportModel);

            var conflictDay = await repo.GetDayAsync(DateTime.Today.AddDays(-1).ToKey());
            Assert.AreEqual(99.0M, conflictDay.Temperature);
            Assert.IsTrue(repo.Settings.LastSyncDate > syncTime, $"Sync Time {repo.Settings.LastSyncDate} should be greater than {syncTime}");
        }


        [TestMethod]
        public async Task Sync_SameDaysSecondaryOlder()
        {
            ChartingRepo repo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            var syncTime = repo.Settings.LastSyncDate;

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 98.6M, ModifiedOn = DateTime.UtcNow.AddHours(-2) });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), });


            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 99.0M, ModifiedOn = DateTime.UtcNow.AddHours(-5) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), });

            await repo.MergeAsync(secondaryRepo.ExportModel);

            var conflictDay = await repo.GetDayAsync(DateTime.Today.AddDays(-1).ToKey());
            Assert.AreEqual(98.6M, conflictDay.Temperature);
            Assert.IsTrue(repo.Settings.LastSyncDate > syncTime, $"Sync Time {repo.Settings.LastSyncDate} should be greater than {syncTime}");
        }

        [TestMethod]
        public async Task Sync_DaysOlderThanLastSync()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() { LastSyncDate = DateTime.UtcNow.AddHours(-2) });
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());

            var syncTime = repo.Settings.LastSyncDate;

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 98.6M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), Temperature = 98.6M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });


            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 99.0M, ModifiedOn = DateTime.UtcNow.AddHours(-5) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), Temperature = 99.0M, ModifiedOn = DateTime.UtcNow.AddHours(-5) });

            await repo.MergeAsync(secondaryRepo.ExportModel);

            var conflictDay = await repo.GetDayAsync(DateTime.Today.AddDays(-1).ToKey());
            Assert.AreEqual(98.6M, conflictDay.Temperature);
            Assert.IsTrue(repo.Settings.LastSyncDate > syncTime, $"Sync Time {repo.Settings.LastSyncDate} should be greater than {syncTime}");
        }

        [TestMethod]
        public async Task Sync_WithMultipleCycles()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() { });
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());

            var syncTime = repo.Settings.LastSyncDate;

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 91M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), Temperature = 92M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), Temperature = 93M, ModifiedOn = DateTime.UtcNow.AddHours(-1) }, startNewCycle: true);
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), Temperature = 94M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });

            var start_cycles = repo.ExportModel.Cycles.Select(q => q.StartDate.ToKey()).ToList();
            Console.WriteLine(string.Join(",", start_cycles));
            CollectionAssert.AreEqual(
                new List<string>() { DateTime.Today.AddDays(-4).ToKey(), DateTime.Today.AddDays(-2).ToKey() },
                repo.ExportModel.Cycles.Select(q => q.StartDate.ToKey()).ToList()
            );
            Assert.AreEqual(2, repo.CycleCount, $"Expected 2 Cycle before merge");

            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(0), Temperature = 99M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 98M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), Temperature = 92M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), Temperature = 93M, ModifiedOn = DateTime.UtcNow.AddHours(-1) }, startNewCycle: true);
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), Temperature = 94M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });


            await repo.MergeAsync(secondaryRepo.ExportModel);

            start_cycles = repo.ExportModel.Cycles.Select(q => q.StartDate.ToKey()).ToList();
            Console.WriteLine(string.Join(",", start_cycles));
            CollectionAssert.AreEqual(
                new List<string>() { DateTime.Today.AddDays(-4).ToKey(), DateTime.Today.AddDays(-2).ToKey() },
                repo.ExportModel.Cycles.Select(q => q.StartDate.ToKey()).ToList()
            );

            var conflictDay = await repo.GetDayAsync(DateTime.Today.AddDays(-1).ToKey());
            Assert.AreEqual(2, repo.CycleCount, $"Expected Cycle after Merge");
            Assert.AreEqual(98M, conflictDay.Temperature);
            Assert.IsTrue(repo.Settings.LastSyncDate > syncTime, $"Sync Time {repo.Settings.LastSyncDate} should be greater than {syncTime}");
        }

        [TestMethod]
        public async Task Sync_EmptySourceWithMultipleCycles()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() { });
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());

            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(0), Temperature = 99M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), Temperature = 98M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-2), Temperature = 92M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-3), Temperature = 93M, ModifiedOn = DateTime.UtcNow.AddHours(-1) }, startNewCycle: true);
            await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-4), Temperature = 94M, ModifiedOn = DateTime.UtcNow.AddHours(-1) });


            await repo.MergeAsync(secondaryRepo.ExportModel);

            Assert.AreEqual(2, repo.CycleCount, $"Expected Cycle after Merge");
            var start_cycles = repo.ExportModel.Cycles.Select(q => q.StartDate.ToKey()).ToList();
            Console.WriteLine(string.Join(",", start_cycles));
            CollectionAssert.AreEqual(
                new List<string>() { DateTime.Today.AddDays(-4).ToKey(), DateTime.Today.AddDays(-2).ToKey() },
                repo.ExportModel.Cycles.Select(q => q.StartDate.ToKey()).ToList()
            );

            Assert.IsTrue(repo.Settings.LastSyncDate > DateTime.MinValue, $"Sync Time {repo.Settings.LastSyncDate} should be greater than {DateTime.MinValue}");
        }
    }
}