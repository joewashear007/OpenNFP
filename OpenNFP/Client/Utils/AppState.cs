﻿namespace OpenNFP.Client.Utils
{
    public class AppState : IAppState
    {
        private Dictionary<string, object?> _state = new Dictionary<string, object?>();

        public event EventHandler<EventArgs> Changed;
        public void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public bool ShowDetailedCharts { get; set; } = true;
        public bool ShowStartUpSync { get; set; } = true;

        public bool ShouldRefresh { get; set; } = true;

        public bool ShouldRunSyncOnStartup { get; set; } = true;

        public int InitialCyclesToLoad { get; set; } = 3;

        public void Set<T>(string key, T? value)
        {
            _state[key] = value;
            NotifyChanged();
        }

        public bool TryGet<T>(string key, out T? value)
        {
            if (_state.TryGetValue(key, out object? v))
            {
                if (v is not null)
                {
                    value = (T)v;
                }
                else
                {
                    value = default;
                }
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
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


        public bool TryGet<T>(string key, out T? value);
        public void Set<T>(string key, T? value);

        public event EventHandler<EventArgs> Changed;
        public void NotifyChanged();
    }
}
