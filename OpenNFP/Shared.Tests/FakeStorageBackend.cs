using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Tests
{
    internal class FakeStorageBackend<U> : IStorageBackend 
    {
        private readonly Dictionary<string, U> _data = new Dictionary<string, U>();
        public Task<T> Read<T>(string key)
        {
            return (T)_data[key];
        }

        public Task Write<T>(string key, T obj)
        {
            return Task.CompletedTask;
        }
    }
}
