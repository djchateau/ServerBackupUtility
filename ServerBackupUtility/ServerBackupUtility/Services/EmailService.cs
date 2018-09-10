
using ServerBackupUtility.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace ServerBackupUtility.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _dateTime = DateTime.Now.ToLocalTime().ToString("yy-MM-dd");
        private readonly bool _emailService = Convert.ToBoolean(ConfigurationManager.AppSettings["EmailService"]);

        public void CreateMessge()
        {
            if (_emailService)
            {
                LogService.LogEvent("Sending Email");

                FileStream fileStream = new FileStream(_path + "\\LogFiles\\" + _dateTime + ".txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);

                try
                {
                    string logFile = streamReader.ReadToEnd();

                    ISmtpService emailService = new SmtpService();
                    emailService.SendMail("\r\n" + logFile + "\r\n");
                }
                catch (Exception ex)
                {
                    LogService.LogEvent("Error: SmtpService.CreateSmtpMessge - " + ex.Message);
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
