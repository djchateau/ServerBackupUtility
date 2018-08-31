
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public interface IServerBackupService
    {
        Task RunServerBackupAsync();
    }
}
