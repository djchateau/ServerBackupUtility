
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ServerBackupUtility.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly string _archivePaths = ConfigurationManager.AppSettings["ArchivePaths"].Trim();
        private readonly string _backupPath = ConfigurationManager.AppSettings["BackupPath"].Trim();

        public void CreateArchives()
        {
            LogService.LogEvent("Reading Archive Folder Paths");
            LogService.LogEvent();

            string[] archivePaths = null;

            if (!String.IsNullOrEmpty(_archivePaths))
            {
                archivePaths = _archivePaths.Split(new [] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                LogService.LogEvent("No Archive Paths Folder Specified - Skipping Folder Archiving");
                return;
            }

            if (String.IsNullOrEmpty(_backupPath))
            {
                LogService.LogEvent("No Backup Path Folder Specified - Skipping Folder Archiving");
                return;
            }

            try
            {
                if (archivePaths.Any())
                {
                    foreach (var archivePath in archivePaths)
                    {
                        IEnumerable<String> directoryPaths = Directory.EnumerateDirectories(archivePath, "*", SearchOption.TopDirectoryOnly);

                        if (directoryPaths.Any())
                        {
                            foreach (var directoryPath in directoryPaths)
                            {
                                int index = directoryPath.LastIndexOf('\\');
                                string directoryName = directoryPath.Substring(index);

                                LogService.LogEvent("Creating Archive: " + directoryName);
                                ZipFile.CreateFromDirectory(directoryPath, _backupPath + directoryName + ".zip", CompressionLevel.Optimal, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: ArchiveService.CreateArchives - " + ex.Message);
            }

            LogService.LogEvent();
            LogService.LogEvent("Finished Archiving Folders");
        }
    }
}
