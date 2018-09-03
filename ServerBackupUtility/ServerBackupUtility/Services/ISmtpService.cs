
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public interface ISmtpService
    {
        Task CreateSmtpMessgeAsync();

    }
}
