
using ServerBackupUtility.Logging;
using System;
using System.IO;
using System.ServiceProcess;

namespace ServerBackupUtility.Services
{
    public class RestartService : IRestartService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;

        public void WatchAppConfig()
        {
            FileSystemWatcher fileWatcher = new FileSystemWatcher();

            fileWatcher.Path = _path;
            fileWatcher.Filter = "*.config";
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.IncludeSubdirectories = false;
            fileWatcher.Changed += FileWatcher_Changed;
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ServiceController serviceController = new ServiceController("BackupScheduler");

            try
            {
                int ms1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(5000);

                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                int ms2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(5000 - (ms2 - ms1));

                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                LogService.LogEvent("RestartService.RestartWindowsService - " + ex.Message);
            }
        }
    }
}
