using Firebase.Storage;
using ProjectIP.Interfaces;
using System;
using Prism.Ioc;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProjectIP.Services
{
    class StorageService : IStorageService
    {
        public IAuthenticationService _authenticationService { get; private set; }
        private FirebaseStorage StorageClient { get; set; }
        public StorageService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            StorageClient = App.Current.Container.Resolve<FirebaseStorage>();
        }
        public async Task AddFile(byte[] fileByteArray, string filename)
        {
            var uid = _authenticationService.GetUid();
            var stream = new MemoryStream(fileByteArray);
            await StorageClient.Child("users").Child(uid).Child(filename).PutAsync(stream);
        }

        public async Task<bool> DeleteFile(string path)
        {
            if (path.Contains("shared"))
            {
                return false;
            }
            await StorageClient.Child(path).DeleteAsync();
            return true;
        }

        public async Task<string> GetFileUrl(string filename)
        {
            var uid = _authenticationService.GetUid();
            return await StorageClient.Child("users").Child(uid).Child(filename).GetDownloadUrlAsync();
        }
    }
}
