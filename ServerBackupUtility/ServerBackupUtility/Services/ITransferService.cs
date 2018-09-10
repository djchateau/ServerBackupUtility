
namespace ServerBackupUtility.Services
{
    public interface ITransferService
    {
        void InitializeFtp();
        void UploadFile(string filePath);
    }
}
