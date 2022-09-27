using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Interfaces
{
    public interface IRemoteStorageBackend 
    {
        Task<SyncInfo> GetLastSyncInfo();
        Task WriteAsync<T>(SyncInfo key, T obj);
        Task<T?> ReadAsync<T>(SyncInfo key);
        Task<MemoryStream> ReadStreamAsync(SyncInfo key);
    }
}
