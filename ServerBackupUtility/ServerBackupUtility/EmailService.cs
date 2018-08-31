﻿
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public class EmailService : IEmailService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _userName = ConfigurationManager.AppSettings["SmtpUserName"];
        private readonly string _password = ConfigurationManager.AppSettings["SmtpPassword"];
        private readonly string _smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
        private readonly int _smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
        private readonly bool _smtpSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSsl"]);
        private readonly string _sender = ConfigurationManager.AppSettings["SmtpSender"];
        private readonly string _recipient = ConfigurationManager.AppSettings["SmtpRecipient"];

        private readonly string _subject = "Daily Backup Report - " + DateTime.Now.ToLongDateString();

        public async Task SendEmailAsync(string messageBody)
        {
            X509Certificate2 certificate2 = new X509Certificate2(_path + "\\localhost.pfx", "secret");

            MailMessage mailMessage = null;
            SmtpClient smtpClient = null;

            try {
                mailMessage = new MailMessage(_sender, _recipient);
                mailMessage.Body = messageBody;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.BodyTransferEncoding = TransferEncoding.EightBit;
                mailMessage.IsBodyHtml = false;
                mailMessage.Subject = _subject;
                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.Priority = MailPriority.Normal;

                NetworkCredential credentials = new NetworkCredential();
                credentials.UserName = _userName;
                credentials.Password = _password;

                smtpClient = new SmtpClient();
                smtpClient.Host = _smtpHost;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = credentials;
                smtpClient.ClientCertificates.Add(certificate2);
                smtpClient.Port = _smtpPort;
                smtpClient.EnableSsl = _smtpSsl;
                smtpClient.DeliveryFormat = SmtpDeliveryFormat.International;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                await LogService.LogSmtpErrorAsync("Error: EmailService.SendEmailAsync - " + ex.Message);
            }
            finally
            {
                if (mailMessage != null)
                {
                    mailMessage.Dispose();
                }

                if (smtpClient != null)
                {
                    smtpClient.Dispose();
                }
            }
        }
    }
}
