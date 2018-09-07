
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public interface IFtpService
    {
        Task InitializeFtpAsync(bool status = false);
        Task UploadFileAsync(string filePath);
        Task DeleteCurrentFtpFolderAsync();
    }
}
