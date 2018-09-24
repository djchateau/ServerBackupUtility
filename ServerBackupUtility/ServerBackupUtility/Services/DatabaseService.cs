
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace ServerBackupUtility.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _databasePath = ConfigurationManager.AppSettings["DatabasePath"].Trim();
        private readonly bool _deleteFiles = Convert.ToBoolean(ConfigurationManager.AppSettings["DeleteFiles"].Trim());

        public void BackupDatabases(ITransferService transferService)
        {
            LogService.LogEvent("Reading Database Files");

            IEnumerable<String> dbFilePaths = null;

            if (!String.IsNullOrEmpty(_databasePath))
            {
                dbFilePaths = Directory.EnumerateFiles(_databasePath, "*", SearchOption.AllDirectories);
            }
            else
            {
                LogService.LogEvent("No Database Folder Specified - Skipping Database Files Backup");
                return;
            }

            try
            {
                if (dbFilePaths.Any())
                {
                    foreach (var dbFilePath in dbFilePaths)
                    {
                        int index = dbFilePath.Trim().LastIndexOf('\\');
                        string dbName = dbFilePath.Trim().Substring(index + 1);

                        LogService.LogEvent("Uploading DataBase To FTP Server: " + dbName);

                        if (transferService.UploadFile(dbFilePath))
                        {
                            Thread.Sleep(1000);
                            if (_deleteFiles) { File.Delete(dbFilePath); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: DatabaseService.BackupDatabases - " + ex.Message);
            }

            LogService.LogEvent("Finiahed Database Files Transfer");
        }
    }
}
