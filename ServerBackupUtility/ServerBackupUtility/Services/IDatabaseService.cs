
namespace ServerBackupUtility.Services
{
    public interface IDatabaseService
    {
        void BackupDatabases(IFtpService ftpService);
    }
}
