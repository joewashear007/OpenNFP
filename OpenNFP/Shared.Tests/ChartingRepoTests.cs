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
        public async Task AddDay_newCycle_SettingsUpdated()
        {
            FakeStorageBackend storageBackend = new();
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());

            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1), ClearBlueResult = ClearBlueResult.Peak, });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-10), ClearBlueResult = ClearBlueResult.Low, });
            await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today, ClearBlueResult = ClearBlueResult.Low, }, startNewCycle: true);

            Assert.AreEqual(2, repo.Cycles.Count());
            // 10 day + today since we don't add today
            Assert.AreEqual(2, ((IEnumerable<Cycle>)storageBackend.Data[ChartingRepo.CYCLE_KEY]).Count());
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

        [TestMethod]
        public async Task Sync_SmallCycles()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() { });
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(1, 25))
            {
                await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow.AddHours(-1) }, startNewCycle: i == 5);
            }

            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(3, 23))
            {
                await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow.AddHours(-1) }, startNewCycle: i == 6);
            }


            await repo.MergeAsync(secondaryRepo.ExportModel);

            Assert.AreEqual(2, repo.CycleCount, $"Expected Cycle after Merge");
            var expected_cycles = new List<string>() { DateTime.Today.AddDays(-25).ToKey(), DateTime.Today.AddDays(-4).ToKey() };
            var start_cycles = repo.ExportModel.Cycles.Select(q => q.StartDate.ToKey()).ToList();
            Console.WriteLine(string.Join(",", start_cycles));
            Console.WriteLine(string.Join(",", expected_cycles));
            CollectionAssert.AreEqual(expected_cycles, start_cycles);

            Assert.IsTrue(repo.Settings.LastSyncDate > DateTime.MinValue, $"Sync Time {repo.Settings.LastSyncDate} should be greater than {DateTime.MinValue}");
        }


        [TestMethod]
        public async Task Sync_KeepEmptyDays()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() { });
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(0, 5))
            {
                await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow.AddHours(-4), Temperature = 97 + i * .1M });
            }

            Console.WriteLine("First Repo Cycles:");
            repo.ExportModel.Records.ForEach(Console.WriteLine);
            Assert.IsTrue(repo.ExportModel.Records.All(q => q.IsEmpty() == false), $"Expected all cycles days to be not empty to start");

            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(0, 5))
            {
                await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow });
            }

            Console.WriteLine("Secondary Repo Cycles:");
            secondaryRepo.ExportModel.Records.ForEach(Console.WriteLine);
            Assert.IsTrue(secondaryRepo.ExportModel.Records.All(q => q.IsEmpty()), $"Expected all secondary cycles days to be empty to start");

            await repo.MergeAsync(secondaryRepo.ExportModel);

            Console.WriteLine("Merged Cycles:");
            repo.ExportModel.Records.ForEach(Console.WriteLine);
            Assert.IsTrue(repo.ExportModel.Records.All(q => q.IsEmpty() == false), $"Expected all cycles days to be not empty at the end");
        }

        [TestMethod]
        public async Task Sync_WithDeletedCycles()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() { });
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(0, 12))
            {
                await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow.AddHours(-4), Temperature = 97 + i * .1M }, startNewCycle:  i == 6);
            }
            await repo.DeleteCycleAsync(DateTime.Today.AddDays(-5).ToKey());

            Console.WriteLine("First Repo Cycles:");
            repo.ExportModel.Cycles.ForEach(Console.WriteLine);
            Assert.AreEqual(1, repo.ExportModel.Cycles.Where(q => !q.Deleted).Count(), $"Expected one cycle with deleted cycles");

            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(0, 12))
            {
                await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow });
            }

            Console.WriteLine("Secondary Repo Cycles:");
            secondaryRepo.ExportModel.Cycles.ForEach(Console.WriteLine);
            Assert.AreEqual(1, repo.ExportModel.Cycles.Where(q => !q.Deleted).Count(), $"Expected one cycle in seconday repo");

            await repo.MergeAsync(secondaryRepo.ExportModel);

            Console.WriteLine("Merged Cycles:");
            repo.ExportModel.Cycles.ForEach(Console.WriteLine);
            Assert.AreEqual(1, repo.CycleCount, "Expected 1 Cycle");
            Assert.AreEqual(DateTime.Today.ToKey(), repo.Cycles.First(q =>!q.Item.Deleted).Item.EndDate.ToKey(), "Expected ");

        }

        [TestMethod]
        public async Task Sync_IncomingDeletedCycles()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() { });
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(0, 12))
            {
                await repo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow.AddHours(-4), Temperature = 97 + i * .1M });
            }

            Console.WriteLine("First Repo Cycles:");
            repo.ExportModel.Cycles.ForEach(Console.WriteLine);
            Assert.AreEqual(1, repo.ExportModel.Cycles.Where(q => !q.Deleted).Count(), $"Expected one cycle with deleted cycles");

            ChartingRepo secondaryRepo = new(new FakeStorageBackend(), new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(0, 12))
            {
                await secondaryRepo.AddUpdateRecord(new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow }, startNewCycle: i == 6);
            }
            await secondaryRepo.DeleteCycleAsync(DateTime.Today.AddDays(-5).ToKey());

            Console.WriteLine("Secondary Repo Cycles:");
            secondaryRepo.ExportModel.Cycles.ForEach(Console.WriteLine);
            Assert.AreEqual(1, repo.ExportModel.Cycles.Where(q => !q.Deleted).Count(), $"Expected one cycle in seconday repo");

            await repo.MergeAsync(secondaryRepo.ExportModel);

            Console.WriteLine("Merged Cycles:");
            repo.ExportModel.Cycles.ForEach(Console.WriteLine);
            Assert.AreEqual(1, repo.CycleCount, "Expected 1 Cycle");
            Assert.AreEqual(DateTime.Today.ToKey(), repo.Cycles.First(q => !q.Item.Deleted).Item.EndDate.ToKey(), "Expected ");

        }


        [TestMethod]
        public async Task RestoreCycle_DeletedShortCycles()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() { });
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());
            foreach (int i in Enumerable.Range(0, 40))
            {
                var rec = new DayRecord { Date = DateTime.Today.AddDays(-1 * i), ModifiedOn = DateTime.UtcNow.AddHours(-4), Temperature = 97 + i * .1M };
                bool newCyle = i == 5 || i == 3;
                await repo.AddUpdateRecord(rec, newCyle);
            }
            await repo.DeleteCycleAsync(DateTime.Today.AddDays(-1 * 4).ToKey());
            await repo.DeleteCycleAsync(DateTime.Today.AddDays(-1 * 2).ToKey());

            var next_rec = new DayRecord { Date = DateTime.Today.AddDays(1), ModifiedOn = DateTime.UtcNow.AddHours(-4), Temperature = 97.1M };
            await repo.AddUpdateRecord(next_rec);

            Console.WriteLine("Current Cycles:");
            repo.GetCycles(skipDeleted: false).Select(q => q.Item).ToList().ForEach(Console.WriteLine);


            Assert.AreEqual(repo.GetCycles(skipDeleted: true).Count(), 1);
            Assert.AreEqual(repo.GetCycles(skipDeleted: false).Count(), 3);

            await repo.RestoreCycleAsync(DateTime.Today.AddDays(-1 * 4).ToKey());

            Console.WriteLine("Updated Cycles:");
            repo.GetCycles(skipDeleted: false).Select(q => q.Item).ToList().ForEach(Console.WriteLine);

            Assert.AreEqual(repo.GetCycles(skipDeleted: true).Count(), 2);
            Assert.AreEqual(repo.GetCycles(skipDeleted: false).Count(), 3);

        }


        [TestMethod]
        public async Task Initiaze()
        {
            FakeStorageBackend storageBackend = new();
            await storageBackend.WriteAsync(ChartingRepo.SETTING_KEY, new ChartSettings() {});
            await storageBackend.WriteAsync(ChartingRepo.CYCLE_KEY, new List<Cycle>() { 
                new Cycle() { StartDate = DateTime.Today.AddDays(-90) },
                new Cycle() { StartDate = DateTime.Today.AddDays(-60) },
                new Cycle() { StartDate = DateTime.Today.AddDays(-45), Deleted = true },
                new Cycle() { StartDate = DateTime.Today.AddDays(-42), Deleted = true },
                new Cycle() { StartDate = DateTime.Today.AddDays(-33), Deleted = true },
                new Cycle() { StartDate = DateTime.Today.AddDays(-30) },
                new Cycle() { StartDate = DateTime.Today.AddDays(-5), Deleted = true }
            });
            var repo = new ChartingRepo (storageBackend, new NullLogger<IChartingRepo>());
            await repo.InitializeAsync();
            Assert.AreEqual(3, repo.CycleCount, "Expected only active secrets");
        }
    }
}