using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared
{
    public interface IStorageBackend
    {
        Task WriteAsync<T>(string key, T obj);

        Task<T> ReadAsync<T>(string key);
    }
}
