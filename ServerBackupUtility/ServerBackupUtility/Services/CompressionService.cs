
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;

namespace ServerBackupUtility.Services
{
    public class CompressionService : ICompressionService
    {
        private readonly string _archivePaths = ConfigurationManager.AppSettings["ArchivePaths"].Trim();
        private readonly string _backupPath = ConfigurationManager.AppSettings["BackupPath"].Trim();

        public void CreateArchives()
        {
            LogService.LogEvent("Reading Archive File Paths");

            string[] archivePaths = _archivePaths.Split(new [] {'|'}, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (var archivePath in archivePaths)
                {
                    IEnumerable<String> directoryPaths = Directory.EnumerateDirectories(archivePath, "*", SearchOption.TopDirectoryOnly);

                    foreach (var directoryPath in directoryPaths)
                    {
                        int index = directoryPath.Trim().LastIndexOf('\\');
                        string directoryName = directoryPath.Trim().Substring(index);

                        LogService.LogEvent("Creating Archive: " + directoryName);
                        ZipFile.CreateFromDirectory(directoryPath, _backupPath + directoryName + ".zip", CompressionLevel.Optimal, true);
                    }
                }
            }
            catch (IOException ex)
            {
                LogService.LogEvent("Error: ArchiveService.CreateArchives - " + ex.Message);
            }

            LogService.LogEvent("Completed Archive File Compression");
        }
    }
}
