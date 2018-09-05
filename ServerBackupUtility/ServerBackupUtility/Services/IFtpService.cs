
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public interface IFtpService
    {
        Task InitializeFtpAsync(bool status = false);
        Task UploadFileAsync(string filePath);
        Task DeleteCurrentFtpFolderAsync();
    }
}
