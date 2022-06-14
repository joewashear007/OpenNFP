using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Internal
{
    internal class CycleRepo
    {
        private readonly IStorageBackend _storage;
        private readonly SortedDictionary<string, int> _cycleDayMap;
        private readonly SortedDictionary<string, Cycle> _knownCycles;

        internal CycleRepo(IStorageBackend storage)
        {
            _storage = storage;
        }

        internal async Task<Cycle> GetCycleAsync(string date)
        {
            if (_knownCycles.TryGetValue(date, out Cycle? cycle))
            {
                return cycle;
            }
            else
            {
                return null;
            }
        }
    }
}
