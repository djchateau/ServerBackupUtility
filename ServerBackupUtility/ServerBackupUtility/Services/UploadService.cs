
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public class UploadService : IUploadService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _archivePath = ConfigurationManager.AppSettings["ArchivePath"];

        public async Task UploadBackupFilesAsync(IFtpService ftpService)
        {
            StreamReader backupFiles = new StreamReader(_path + "\\ServerBackupFiles.txt", Encoding.UTF8);
            ICollection<String> backupPaths = null;

            await LogService.LogEventAsync("Reading Backup File Paths");

            try
            {
                string line;

                backupPaths = new Collection<String>();

                while ((line = await backupFiles.ReadLineAsync()) != null)
                {
                    backupPaths.Add(line);
                }

                backupPaths.Add(_archivePath);
            }
            catch (Exception ex)
            {
                await LogService.LogEventAsync("Error: UploadService.UploadBackupFilesAsync #1 - " + ex.Message);
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

                        foreach (var filePath in Directory.EnumerateFiles(folderPath, filePattern, SearchOption.AllDirectories))
                        {
                            int index2 = filePath.Trim().LastIndexOf('\\');
                            string fileName = filePath.Trim().Substring(index2 + 1);

                            await LogService.LogEventAsync("Uploading Backup Files To FTP Server: " + fileName);
                            await ftpService.UploadFileAsync(filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await LogService.LogEventAsync("Error: UploadService.UploadBackupFilesAsync #2 - " + ex.Message);
            }

            await LogService.LogEventAsync("Finishing Backup Files Process");
        }
    }
}
