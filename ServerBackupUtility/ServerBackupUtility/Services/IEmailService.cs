
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string messageBody);
    }
}
