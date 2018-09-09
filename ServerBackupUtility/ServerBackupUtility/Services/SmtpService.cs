
using ServerBackupUtility.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public class SmtpService : ISmtpService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _dateTime = DateTime.Now.ToString("yy-MM-dd");
        private readonly bool _smtpService = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpService"]);

        public async Task CreateSmtpMessgeAsync()
        {
            if (_smtpService)
            {
                await LogService.LogEventAsync("Sending Email");

                FileStream fileStream = new FileStream(_path + "\\LogFiles\\" + _dateTime + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);

                try
                {
                    string logFile = await streamReader.ReadToEndAsync();

                    IEmailService emailService = new EmailService();
                    await emailService.SendEmailAsync("\r\n" + logFile + "\r\n");
                }
                catch (Exception ex)
                {
                    await LogService.LogEventAsync("Error: SmtpService.CreateSmtpMessgeAsync - " + ex.Message);
                }
                finally
                {
                    streamReader.Close();

                    fileStream.Close();
                }
            }
        }
    }
}
