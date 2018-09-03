
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public interface IDatabaseService
    {
        Task BackupDatabasesAsync(IFtpService ftpService);
    }
}
