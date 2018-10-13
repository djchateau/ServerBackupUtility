
using System;
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public interface ITransferService
    {
        Task<Boolean> InitializeFtpAsync();
        Task<Boolean> UploadFileAsync(string filePath);
    }
}
