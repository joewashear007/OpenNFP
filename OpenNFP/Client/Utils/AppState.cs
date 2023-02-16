namespace OpenNFP.Client.Utils
{
    public class AppState: IAppState
    {
        public bool ShowDetailedCharts { get; set; } = true;
        public bool ShowStartUpSync { get; set; } = true;

        public bool ShouldRefresh { get; set; } = true;

        public bool ShouldRunSyncOnStartup { get; set; } = true;

        public int InitialCyclesToLoad { get; set; } = 3;
    }

    public interface IAppState
    {
        /// <summary>
        /// Run the sync on app startup
        /// </summary>
        bool ShouldRunSyncOnStartup { get; set; }

        /// <summary>
        /// The number of cycles to load on the cycle list screen
        /// </summary>
        int InitialCyclesToLoad { get; set; }
        
    }
}
