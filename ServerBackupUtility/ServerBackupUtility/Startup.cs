
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;

namespace ServerBackupUtility
{
    public partial class Startup : ServiceBase
    {
        public Startup()
        {
            InitializeComponent();
        }

        private Timer _scheduler = null;
        private DateTime _time = DateTime.Parse(ConfigurationManager.AppSettings["Time"]);

        protected override void OnStart(string[] args)
        {
            WriteToLog("Scheduler Service Started");
            SchedulerService();
        }

        protected override void OnStop()
        {
            WriteToLog("Scheduler Service Stopped");
            _scheduler.Dispose();
        }

        public void SchedulerService()
        {
            try
            {
                _scheduler = new Timer(new TimerCallback(SchedulerCallback));

                if (DateTime.Now.ToLocalTime() > _time)
                {
                    // If scheduled time is passed, set schedule for the next day.
                    _time = _time.AddDays(1);
                }

                TimeSpan timeSpan = _time.Subtract(DateTime.Now.ToLocalTime());
                string schedule = String.Format("{0} days {1} hours {2} minutes {3} seconds", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                WriteToLog(String.Format("Scheduler Service Scheduled to Run in: {0}", schedule));

                // Get the difference in minutes between the scheduled time and the current time.
                int minutes = Convert.ToInt32(timeSpan.TotalMilliseconds);

                // Change the timer's due time.
                _scheduler.Change(minutes, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                WriteToLog(String.Format("Scheduler Service Error: {0}", ex.Message));

                // Stop the Windows service.
                using (var serviceController = new ServiceController("BackupScheduler"))
                {
                    serviceController.Stop();
                }
            }
        }

        private void SchedulerCallback(object e)
        {
            WriteToLog("Begin Backup Session");
            BackupController backupController = new BackupController();
            backupController.RunBackup();
        }

        private void WriteToLog(string message)
        {
            LogService.CreateLogAsync("Daily Log Created").ConfigureAwait(false);
            LogService.LogEventAsync(message).ConfigureAwait(false);
        }
    }
}
