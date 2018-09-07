
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerBackupUtility.Logging
{
    public static class LogService
    {
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string DateTime = System.DateTime.Now.ToString("yy-MM-dd");

        public static async Task CreateLogAsync(string message)
        {
            if (!Directory.Exists(Path + "\\LogFiles"))
            {
                Directory.CreateDirectory(Path + "\\LogFiles");
            }

            FileStream fileStream = null;

            try
            {
                if (!File.Exists(Path + "\\LogFiles\\" + DateTime + ".txt"))
                {
                    fileStream = new FileStream(Path + "\\LogFiles\\" + DateTime + ".txt", FileMode.Create, FileAccess.Write, FileShare.Write);

                    string formattedMessage = System.DateTime.Now.ToString("G") + " - " + message + "\r\n\r\n";
                    byte[] messageBytes = Encoding.UTF8.GetBytes(formattedMessage);

                    await fileStream.WriteAsync(messageBytes, 0, messageBytes.Length);
                }
            }
            catch (Exception ex)
            {
                await LogEventAsync("Error: LogService.CreateLogAsync - " + ex.Message);
            }
            finally
            {
                fileStream.Close();
            }
        }

        public static Task LogEventAsync()
        {
            return LogEventAsync(null);
        }

        public static async Task LogEventAsync(string message)
        {
            FileStream fileStream = new FileStream(Path + "\\LogFiles\\" + DateTime + ".txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            try
            {
                string formattedMessage = null;

                if (message == null)
                {
                    formattedMessage = "\r\n";
                }
                else
                {
                    formattedMessage = System.DateTime.Now.ToString("G") + " - " + message + "\r\n";
                }

                byte[] messageBytes = Encoding.UTF8.GetBytes(formattedMessage);

                await fileStream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                await LogEventAsync("Error: LogService.LogEventAsync - " + ex.Message);
            }
            finally
            {
                fileStream.Close();
            }
        }

        public static async Task LogSmtpErrorAsync(string message)
        {
            FileStream fileStream = new FileStream(Path + "\\LogFiles\\SmtpErrorLog.txt", FileMode.Append, FileAccess.Write, FileShare.Read);

            try
            {
                string formattedMessage = System.DateTime.Now.ToString("G") + " - " + message + "\r\n\r\n";
                byte[] messageBytes = Encoding.UTF8.GetBytes(formattedMessage);

                await fileStream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                await LogEventAsync("Error: LogService.LogSmtpErrorAsync - " + ex.Message);
            }
            finally
            {
                fileStream.Close();
            }
        }
    }
}
