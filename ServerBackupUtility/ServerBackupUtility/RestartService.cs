
using System;
using System.IO;
using System.ServiceProcess;

namespace ServerBackupUtility
{
    public class RestartService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private void FileSystemEventHandler(object sender, FileSystemEventArgs e) => RestartWindowsService("BackupScheduler");

        public void WatchAppConfig()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = _path;
            watcher.Filter = "App.Config";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = false;
            watcher.Changed += FileSystemEventHandler;
        }

        private void RestartWindowsService(string serviceName)
        {
            ServiceController serviceController = new ServiceController(serviceName);

            try
            {
                if (serviceController.Status.Equals(ServiceControllerStatus.Running) || serviceController.Status.Equals(ServiceControllerStatus.StartPending))
                {
                    serviceController.Stop();
                }

                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch (Exception ex)
            {
                LogService.LogEventAsync("RestartService.RestartWindowsService - " + ex.Message).ConfigureAwait(false);
            }
        }
    }
}
