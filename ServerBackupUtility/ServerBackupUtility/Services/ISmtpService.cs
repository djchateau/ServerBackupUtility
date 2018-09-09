
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public interface ISmtpService
    {
        Task CreateSmtpMessgeAsync();

    }
}
