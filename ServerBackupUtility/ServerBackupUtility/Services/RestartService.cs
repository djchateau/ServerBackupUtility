
using ServerBackupUtility.Logging;
using System;
using System.Diagnostics;
using System.IO;

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

        public void RestartScheduler()
        {
            Process process = new Process();

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();

                processStartInfo.WorkingDirectory = _path;
                processStartInfo.FileName = "RestartScheduler.bat";
                processStartInfo.CreateNoWindow = true;
                processStartInfo.LoadUserProfile = false;
                processStartInfo.UseShellExecute = false;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = processStartInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: RestartService.RestartScheduler - " + ex.Message);
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
            }
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Process process = new Process();

            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();

                processStartInfo.WorkingDirectory = _path;
                processStartInfo.FileName = "RestartScheduler.bat";
                processStartInfo.CreateNoWindow = true;
                processStartInfo.LoadUserProfile = false;
                processStartInfo.UseShellExecute = false;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                process.StartInfo = processStartInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: RestartService.FileWatcher_Changed - " + ex.Message);
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
            }
        }
    }
}
