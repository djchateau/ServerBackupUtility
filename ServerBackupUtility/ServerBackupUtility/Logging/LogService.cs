
using System;
using System.IO;
using System.Text;

namespace ServerBackupUtility.Logging
{
    public static class LogService
    {
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string DateTime = System.DateTime.Now.ToString("yy-MM-dd");

        public static void CreateLog(string message)
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

                    fileStream.Write(messageBytes, 0, messageBytes.Length);
                }
            }
            catch (Exception ex)
            {
                LogEvent("Error: LogService.CreateLog - " + ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        public static void LogEvent()
        {
            LogEvent(null);
        }

        public static void LogEvent(string message)
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

                fileStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                LogEvent("Error: LogService.LogEvent - " + ex.Message);
            }
            finally
            {
                fileStream.Close();
            }
        }

        public static void LogSmtpError(string message)
        {
            FileStream fileStream = new FileStream(Path + "\\LogFiles\\SmtpErrorLog.txt", FileMode.Append, FileAccess.Write, FileShare.Read);

            try
            {
                string formattedMessage = System.DateTime.Now.ToString("G") + " - " + message + "\r\n\r\n";
                byte[] messageBytes = Encoding.UTF8.GetBytes(formattedMessage);

                fileStream.Write(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                LogEvent("Error: LogService.LogSmtpError - " + ex.Message);
            }
            finally
            {
                fileStream.Close();
            }
        }
    }
}
