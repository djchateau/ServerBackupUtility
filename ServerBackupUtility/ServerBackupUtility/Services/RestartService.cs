
using ServerBackupUtility.Logging;
using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;

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
                if (serviceController.Status.Equals(ServiceControllerStatus.Running) || serviceController.Status.Equals(ServiceControllerStatus.StartPending))
                {
                    serviceController.Stop();
                    Thread.Sleep(5000);
                }

                serviceController.Start();
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                LogService.LogEvent("RestartService.RestartWindowsService - " + ex.Message);
            }
        }
    }
}
