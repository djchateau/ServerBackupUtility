
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _databasePath = ConfigurationManager.AppSettings["DatabasePath"];

        public async Task BackupDatabasesAsync(IFtpService ftpService)
        {
            await LogService.LogEventAsync("Reading Database File Paths");

            IEnumerable<String> dbFilePaths = Directory.EnumerateFiles(_databasePath, "*", SearchOption.AllDirectories);

            try
            {
                foreach (var dbFilePath in dbFilePaths)
                {
                    int index = dbFilePath.Trim().LastIndexOf('\\');
                    string dbName = dbFilePath.Trim().Substring(index + 1);

                    await LogService.LogEventAsync("Uploading DataBase To FTP Server: " + dbName);
                    await ftpService.UploadFileAsync(dbFilePath);
                    await Task.Delay(1000);
                    File.Delete(dbFilePath);
                }
            }
            catch (Exception ex)
            {
                await LogService.LogEventAsync("Error: DatabaseService.BackupDatabasesAsync - " + ex.Message);
            }

            await LogService.LogEventAsync("Finishing Database Backup Process");
        }
    }
}
