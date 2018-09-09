
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Text;

namespace ServerBackupUtility.Services
{
    public class UploadService : IUploadService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _archivePath = ConfigurationManager.AppSettings["ArchivePath"];

        public void UploadBackupFiles(IFtpService ftpService)
        {
            StreamReader backupFiles = new StreamReader(_path + "\\ServerBackupFiles.txt", Encoding.UTF8);
            ICollection<String> backupPaths = null;

            LogService.LogEvent("Reading Backup File Paths");

            try
            {
                string line;

                backupPaths = new Collection<String>();

                while ((line = backupFiles.ReadLine()) != null)
                {
                    backupPaths.Add(line);
                }

                backupPaths.Add(_archivePath);
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: UploadService.UploadBackupFilesAsync #1 - " + ex.Message);
            }
            finally
            {
                backupFiles.Close();
            }

            try
            {
                if (backupPaths != null)
                {
                    foreach (var backupPath in backupPaths)
                    {
                        int index1 = backupPath.Trim().LastIndexOf('\\');
                        string filePattern = backupPath.Trim().Substring(index1 + 1);
                        string folderPath = backupPath.Trim().Substring(0, index1);

                        IEnumerable<String> filePaths = Directory.EnumerateFiles(folderPath, filePattern, SearchOption.AllDirectories);

                        foreach (var filePath in filePaths)
                        {
                            int index2 = filePath.Trim().LastIndexOf('\\');
                            string fileName = filePath.Trim().Substring(index2 + 1);

                            LogService.LogEvent("Uploading Backup Files To FTP Server: " + fileName);
                            ftpService.UploadFile(filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: UploadService.UploadBackupFilesAsync #2 - " + ex.Message);
            }

            LogService.LogEvent("Finishing Backup Files Process");
        }
    }
}
