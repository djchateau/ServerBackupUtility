
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public interface IArchiveService
    {
        Task CreateArchivesAsync();
    }
}
