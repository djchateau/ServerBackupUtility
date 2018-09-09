
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly string _folderPaths = ConfigurationManager.AppSettings["FolderPaths"];
        private readonly string _archivePath = ConfigurationManager.AppSettings["ArchivePath"];

        public async Task CreateArchivesAsync()
        {
            await LogService.LogEventAsync("Reading Web Folder Paths");

            string[] folderPaths = _folderPaths.Split(new [] {'|'}, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (var folderPath in folderPaths)
                {
                    IEnumerable<String> directoryPaths = Directory.EnumerateDirectories(folderPath, "*", SearchOption.TopDirectoryOnly);

                    foreach (var directoryPath in directoryPaths)
                    {
                        int index = directoryPath.Trim().LastIndexOf('\\');
                        string directoryName = directoryPath.Trim().Substring(index);

                        await LogService.LogEventAsync("Creating Archive: " + directoryName);

                        if (File.Exists(_archivePath + directoryName + ".zip"))
                        {
                            File.Delete(_archivePath + directoryName + ".zip");
                            Thread.Sleep(1000);
                        }

                        ZipFile.CreateFromDirectory(directoryPath, _archivePath + directoryName + ".zip", CompressionLevel.Optimal, true));
                    }
                }
            }
            catch (Exception ex)
            {
                await LogService.LogEventAsync("Error: ArchiveService.CreateArchivesAsync - " + ex.Message);
            }

            await LogService.LogEventAsync("Finishing Archive Backup Process");
        }
    }
}
