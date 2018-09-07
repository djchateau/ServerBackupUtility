
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public interface IArchiveService
    {
        Task CreateArchivesAsync();
    }
}
