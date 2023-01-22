using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using OpenNFP.Shared.Interfaces;
using OpenNFP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace OpenNFP.Shared.Backend
{
    public class GoogleStorageBackend : IRemoteStorageBackend, IDisposable
    {
        internal class AuthTokenHttpInitializer : IConfigurableHttpClientInitializer
        {
            public AuthTokenHttpInitializer(string token)
            {
                Token = token;
            }
            private string Token { get; set; } = string.Empty;

            void IConfigurableHttpClientInitializer.Initialize(ConfigurableHttpClient httpClient)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
            }
        }

        private DriveService? driveService { get; set; }
        public IAccessTokenProvider TokenProvider { get; }
        public NavigationManager NavigationManager { get; }

        private readonly AccessTokenRequestOptions options = new()
        {
            Scopes = new List<string>() { "openid", "profile", "email", "https://www.googleapis.com/auth/drive.file" }
        };
        private bool disposedValue;

        public string FileName { get; set; } = "opennfp.json";

        public GoogleStorageBackend(IAccessTokenProvider tokenProvider, NavigationManager navigationManager)
        {
            TokenProvider = tokenProvider;
            NavigationManager = navigationManager;
        }



        public async Task<SyncInfo> GetLastSyncInfo(CancellationToken token)
        {
            DriveService service = await GetDriveService(token);

            var listRequest = service.Files.List();
            listRequest.Q = $"name='{FileName}'";
            listRequest.Fields = "files(id,name,createdTime,modifiedTime,size)";
            var rawfiles = await listRequest.ExecuteAsync(token);
            var file = rawfiles.Files.FirstOrDefault();
            if (file != null)
            {
                return new SyncInfo()
                {
                    FileId = file.Id,
                    FileName = file.Name,
                    FileSizeKb = (int)(file.Size ?? 0 / 1024),
                    SyncTimeStamp = file.ModifiedTime.GetValueOrDefault()
                };
            }
            else
            {
                return SyncInfo.Empty;
            }


        }

        public async Task<T?> ReadAsync<T>(SyncInfo key, CancellationToken token)
        {
            MemoryStream fileContents = await ReadStreamAsync(key, token);
            T? data = JsonSerializer.Deserialize<T>(fileContents.ToArray());
            return data;

        }

        public async Task<MemoryStream> ReadStreamAsync(SyncInfo key, CancellationToken token)
        {
            using MemoryStream fileContents = new();
            DriveService? service = await GetDriveService(token);

            var request = service.Files.Get(key.FileId);
            var progress = await request.DownloadAsync(fileContents, token);
            if (progress.Status == DownloadStatus.Failed)
            {
                throw new InvalidOperationException("Failed to download Google Drive File", progress.Exception);
            }
            else
            {
                return fileContents;
            }
        }

        public async Task WriteAsync<T>(SyncInfo key, T obj, CancellationToken token)
        {
            using var stream = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(obj));
            DriveService? service = await GetDriveService(token);
            
            var fileMetadata = new Google.Apis.Drive.v3.Data.File() { Name = key.FileName };

            ResumableUpload<Google.Apis.Drive.v3.Data.File, Google.Apis.Drive.v3.Data.File> request;
            if (!string.IsNullOrEmpty(key.FileId))
            {
                var uploadrequest = service.Files.Update(fileMetadata, key.FileId, stream, "application/json");
                request = uploadrequest;
            }
            else
            {
                var createrequest = service.Files.Create(fileMetadata, stream, "application/json");
                createrequest.Fields = "id";
                request = createrequest;
            }

            IUploadProgress uploadRsq = await request.UploadAsync(token);

        }

        private async Task<DriveService> GetDriveService(CancellationToken token)
        {
            if (driveService == null)
            {
                var tokenResult = await TokenProvider.RequestAccessToken(options);
                if (tokenResult.TryGetToken(out var authToken))
                {
                    token.ThrowIfCancellationRequested();
                    driveService = new DriveService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = new AuthTokenHttpInitializer(authToken.Value),
                        ApplicationName = "OpenNFP",
                        GZipEnabled = false
                    });
                }
                else
                {
                    throw new AccessTokenNotAvailableException(NavigationManager, tokenResult, options.Scopes);
                }
            }
            return driveService;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    driveService?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
