
namespace ServerBackupUtility.Services
{
    public interface IUploadService
    {
        void UploadBackupFiles(IFtpService ftpService);
    }
}
