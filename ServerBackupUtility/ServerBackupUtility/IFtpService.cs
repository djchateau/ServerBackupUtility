
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public interface IFtpService
    {
        Task InitializeFtpAsync();
        Task UploadFileAsync(string filePath);
    }
}
