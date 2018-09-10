
namespace ServerBackupUtility.Services
{
    public interface IDirectUploadService
    {
        void UploadBackupFiles(ITransferService transferService);
    }
}
