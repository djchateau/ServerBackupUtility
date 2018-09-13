
namespace ServerBackupUtility.Services
{
    public interface IDatabaseService
    {
        void BackupDatabases(ITransferService transferService);
    }
}
