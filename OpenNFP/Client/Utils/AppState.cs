namespace OpenNFP.Client.Utils
{
    public class AppState: IAppState
    {
        public bool ShowDetailedCharts { get; set; } = true;
        public bool ShowStartUpSync { get; set; } = true;

        public bool ShouldRefresh { get; set; } = true;

        public bool ShouldRunSyncOnStartup { get; set; } = true;
    }

    public interface IAppState
    {
        bool ShouldRunSyncOnStartup { get; set; }
        
    }
}
