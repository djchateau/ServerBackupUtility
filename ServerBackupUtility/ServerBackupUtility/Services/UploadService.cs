
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerBackupUtility.Services
{
    public class UploadService : IUploadService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _backupPath = ConfigurationManager.AppSettings["BackupPath"].Trim();
        private readonly bool _deleteFiles = Convert.ToBoolean(ConfigurationManager.AppSettings["DeleteFiles"].Trim());

        public void UploadBackupFiles(ITransferService transferService)
        {
            StreamReader backupFiles = new StreamReader(_path + "\\DirectBackupPaths.config", Encoding.UTF8);
            ICollection<String> backupPaths = null;

            LogService.LogEvent("Reading Backup Files");

            try
            {
                string line;
                backupPaths = new Collection<String>();

                while ((line = backupFiles.ReadLine()) != null)
                {
                    backupPaths.Add(line);
                }

                backupPaths.Add(_backupPath + "\\*");
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: UploadService.UploadBackupFiles #1 - " + ex.Message);
            }
            finally
            {
                backupFiles.Close();
            }

            try
            {
                if (backupPaths.Any())
                {
                    foreach (var backupPath in backupPaths)
                    {
                        int index1 = backupPath.Trim().LastIndexOf('\\');
                        string filePattern = backupPath.Trim().Substring(index1 + 1);
                        string folderPath = backupPath.Trim().Substring(0, index1);

                        IEnumerable<String> filePaths = Directory.EnumerateFiles(folderPath, filePattern, SearchOption.AllDirectories);

                        if (filePaths.Any())
                        {
                            foreach (var filePath in filePaths)
                            {
                                string fileName = Path.GetFileName(filePath);
                                LogService.LogEvent("Uploading Backup Files To FTP Server: " + fileName);

                                if (transferService.UploadFile(filePath))
                                {
                                    Thread.Sleep(1000);
                                    if (_deleteFiles) { File.Delete(filePath); }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: UploadService.UploadBackupFiles #2 - " + ex.Message);
            }

            LogService.LogEvent("Finished Backup Files Transfer");
        }
    }
}
