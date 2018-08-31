
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public interface IEmailService
    {
        Task SendEmailAsync(string messageBody);
    }
}
