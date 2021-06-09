using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectIP.Interfaces
{
    public interface IStorageService
    {
        Task AddFile(byte[] fileByteArray, string filename);
        Task<bool> DeleteFile(string path);
        Task<string> GetFileUrl(string filename);
    }
}
