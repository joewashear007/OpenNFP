using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Internal
{
    internal class DayRepo
    {
        private readonly IStorageBackend _storage;
        private readonly SortedDictionary<string, DayRecord> _data;

        public DayRepo(IStorageBackend storage)
        {
            _data = new SortedDictionary<string, DayRecord>();
            _storage = storage;
        }

        /// <summary>
        /// Get DayRecord from dict or storage 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal async Task<DayRecord> GetAsync(string key)
        {
            if (_data.TryGetValue(key, out DayRecord? day))
            {
                if (day == null)
                {
                    day = new DayRecord(key);
                    await _storage.WriteAsync(day.IndexKey, day);
                    return day;
                }
                else
                {
                    return day;
                }
            }
            else
            {
                DayRecord loadDay = await _storage.ReadAsync<DayRecord>(key);
                if (loadDay != null)
                {
                    _data[key] = loadDay;
                    return loadDay;
                }
                else
                {
                    day = new DayRecord(key);
                    await _storage.WriteAsync(day.IndexKey, day);
                    return day;
                }
            }
        }

        /// <summary>
        /// Get DayRecord from dict or storage 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal async Task<DayRecord> GetAsync(DateTime date)
        {
            return await GetAsync(date.ToKey());
        }

        /// <summary>
        /// Check for existance from dict or storage
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal async Task<bool> ExistsAsync(string key)
        {
            if (_data.ContainsKey(key))
            {
                return true;
            }
            else
            {
                DayRecord loadDay = await _storage.ReadAsync<DayRecord>(key);
                if (loadDay != null)
                {
                    _data[key] = loadDay;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Set the dict and the local storage
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        internal async Task SetAsync(DayRecord day)
        {
            _data[day.IndexKey] = day;
            await _storage.WriteAsync(day.IndexKey, day);
        }

        internal void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// All The Day Records
        /// </summary>
        internal IEnumerable<DayRecord> Values => _data.Values;
    }
}
