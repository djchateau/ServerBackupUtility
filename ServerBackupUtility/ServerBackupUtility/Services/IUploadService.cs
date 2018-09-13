
namespace ServerBackupUtility.Services
{
    public interface IUploadService
    {
        void UploadBackupFiles(ITransferService transferService);
    }
}
