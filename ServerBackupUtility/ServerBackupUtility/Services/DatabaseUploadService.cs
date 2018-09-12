﻿
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;

namespace ServerBackupUtility.Services
{
    public class DatabaseUploadService : IDatabaseUploadService
    {
        private readonly string _databasePath = ConfigurationManager.AppSettings["DatabasePath"].Trim();
        private readonly bool _deleteFiles = Convert.ToBoolean(ConfigurationManager.AppSettings["DeleteFiles"].Trim());

        public void BackupDatabases(ITransferService transferService)
        {
            LogService.LogEvent("Reading Database File Paths");

            IEnumerable<String> dbFilePaths = Directory.EnumerateFiles(_databasePath, "*", SearchOption.AllDirectories);

            try
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
            catch (Exception ex)
            {
                LogService.LogEvent("Error: DatabaseService.BackupDatabases - " + ex.Message);
            }

            LogService.LogEvent("Finishing Database Backup Process");
        }
    }
}
