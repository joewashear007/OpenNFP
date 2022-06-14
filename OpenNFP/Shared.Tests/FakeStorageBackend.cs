using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Tests
{
    internal class FakeStorageBackend : IStorageBackend 
    {
        private readonly Dictionary<string, object> _data = new();

        public Dictionary<string, object> Data => _data;

        public Task<T> ReadAsync<T>(string key)
        {
            return Task.FromResult((T)_data.GetValueOrDefault(key)); ;
        }

        public Task WriteAsync<T>(string key, T obj)
        {
            _data[key] = obj;
            return Task.CompletedTask;
        }
    }
}
