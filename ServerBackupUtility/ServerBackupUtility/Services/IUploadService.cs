
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public interface IUploadService
    {
        Task UploadBackupFilesAsync(IFtpService ftpService);
    }
}
