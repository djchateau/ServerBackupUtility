
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
        private readonly string _dateTime = DateTime.Now.ToString("yy-MM-dd");
        private readonly bool _emailService = Convert.ToBoolean(ConfigurationManager.AppSettings["EmailService"].Trim());

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
                    emailService.SendMailAsync("\r\n" + logFile + "\r\n").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogService.LogEvent("Error: SmtpService.CreateSmtpMessge - " + ex.Message);
                }
                finally
                {
                    if (streamReader != null)
                    {
                        streamReader.Close();
                    }

                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
        }
    }
}
