
namespace ServerBackupUtility.Services
{
    public interface IFtpService
    {
        void InitializeFtp();
        void UploadFile(string filePath);
    }
}
