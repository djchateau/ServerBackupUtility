
using ServerBackupUtility.Logging;
using System;
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

            try
            {
                foreach (var dbFilePath in Directory.EnumerateFiles(_databasePath, "*", SearchOption.AllDirectories))
                {
                    int index = dbFilePath.Trim().LastIndexOf('\\');
                    string dbName = dbFilePath.Trim().Substring(index + 1);

                    await LogService.LogEventAsync("Uploading DataBase To FTP Server: " + dbName);
                    await ftpService.UploadFileAsync(dbFilePath);
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
