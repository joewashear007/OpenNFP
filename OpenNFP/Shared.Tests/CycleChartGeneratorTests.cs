namespace OpenNFP.Shared.Tests
{
    [TestClass]
    public class CycleChartGeneratorTests
    {
        [TestMethod]
        public async Task GetTracesAsync_LoadData_Success()
        {
            FakeStorageBackend storageBackend = new();
            ChartingRepo repo = new(storageBackend, new NullLogger<IChartingRepo>());
            await DemoData.LoadDemoData(repo, DateTime.Today);

            CycleChartGenerator generator = new(repo);

            var vm = await generator.GetTracesAsync(DateTime.Today.AddDays(-10), false);

            var days = Enumerable.Range(0, 11).Reverse().Select(q => DateTime.Today.AddDays(q * -1).ToString("yyyy-MM-dd")).Cast<object>().ToList();
            CollectionAssert.AreEqual(days, vm.Days, "Days failed to match");

            var monitor = new List<ClearBlueResult> {
                ClearBlueResult.Low,
                ClearBlueResult.Low,
                ClearBlueResult.Unknown,
                ClearBlueResult.Low,
                ClearBlueResult.Low,
                ClearBlueResult.Low,
                ClearBlueResult.High,
                ClearBlueResult.High,
                ClearBlueResult.Peak,
                ClearBlueResult.Peak,
                ClearBlueResult.Unknown,
            };

            CollectionAssert.AreEqual(monitor.Select(q => q.ToChartValue()).ToList(), vm.Monitor.Values, "Monitor Values Failed");
            CollectionAssert.AreEqual(monitor.Select(q => q == ClearBlueResult.Unknown ? "" : q.ToString()[..1]).ToList(), vm.Monitor.Annotations, "Monitor Annotions failed");

            var temps = new List<object?>() { 98.6M, 98.2M, 98.3M, 98.4M, 98.7M, 98.9M, 98.6M, 98.4M, 99.0M, 99.9M, null };
            CollectionAssert.AreEqual(temps, vm.Temp.Values, "Temps failure");

        }
    }
}
