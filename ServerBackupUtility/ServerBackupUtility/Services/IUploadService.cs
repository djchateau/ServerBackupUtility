
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public interface IUploadService
    {
        Task UploadBackupFilesAsync(IFtpService ftpService);
    }
}
