
namespace ServerBackupUtility.Services
{
    public interface IDatabaseUploadService
    {
        void BackupDatabases(ITransferService transferService);
    }
}
