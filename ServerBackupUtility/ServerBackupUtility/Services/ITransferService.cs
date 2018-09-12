
namespace ServerBackupUtility.Services
{
    public interface ITransferService
    {
        bool InitializeFtp();
        bool UploadFile(string filePath);
    }
}
