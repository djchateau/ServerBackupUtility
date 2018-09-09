
namespace ServerBackupUtility.Services
{
    public interface IFtpService
    {
        void InitializeFtp(bool status = false);
        void UploadFile(string filePath);
        void DeleteCurrentFtpFolder();
    }
}
