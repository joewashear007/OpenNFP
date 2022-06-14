using Blazored.LocalStorage;
using OpenNFP.Shared;

namespace OpenNFP.Client
{
    public class LocalStorageBackend : IStorageBackend
    {
        private readonly ILocalStorageService _localStorage;

        public LocalStorageBackend(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<T> ReadAsync<T>(string key)
        {
            return await _localStorage.GetItemAsync<T>(key);
        }

        public async Task WriteAsync<T>(string key, T obj)
        {
            await _localStorage.SetItemAsync(key, obj);
        }
    }
}
