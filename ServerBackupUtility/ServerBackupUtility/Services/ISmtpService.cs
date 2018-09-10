
namespace ServerBackupUtility.Services
{
    public interface ISmtpService
    {
        void SendMail(string messageBody);
    }
}
