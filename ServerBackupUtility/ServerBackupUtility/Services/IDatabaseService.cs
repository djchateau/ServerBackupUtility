
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public interface IDatabaseService
    {
        Task BackupDatabasesAsync(IFtpService ftpService);
    }
}
