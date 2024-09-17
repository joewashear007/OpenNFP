using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Interfaces
{
    public interface IRemoteStorageBackEnd 
    {
        Task<SyncInfo> GetLastSyncInfo(string filename, CancellationToken token);
        Task WriteAsync<T>(SyncInfo key, T obj, CancellationToken token);
        Task<T?> ReadAsync<T>(SyncInfo key, CancellationToken token);
        Task<MemoryStream> ReadStreamAsync(SyncInfo key, CancellationToken token);
    }
}
