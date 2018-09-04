
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

        private Timer _schedular = null;
        private DateTime _time = DateTime.Parse(ConfigurationManager.AppSettings["Time"]);

        protected override void OnStart(string[] args)
        {
            WriteToLog("Schedule Service Started {0}");
            ScheduleService();
        }

        protected override void OnStop()
        {
            WriteToLog("Schedule Service Stopped {0}");
            _schedular.Dispose();
        }

        public void ScheduleService()
        {
            try
            {
                _schedular = new Timer(new TimerCallback(SchedularCallback));

                if (DateTime.Now.ToLocalTime() > _time)
                {
                    // If scheduled time is passed, set schedule for the next day.
                    _time = _time.AddDays(1);
                }

                TimeSpan timeSpan = _time.Subtract(DateTime.Now.ToLocalTime());
                string schedule = String.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                WriteToLog("Schedule Service Scheduled to Run at: " + schedule + " {0}");

                // Get the difference in minutes between the scheduled time and the current time.
                int minutes = Convert.ToInt32(timeSpan.TotalMilliseconds);

                // Change the timer's due time.
                _schedular.Change(minutes, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                WriteToLog("Schedule Service Error on: {0} " + ex.Message);

                // Stop the Windows service.
                using (var serviceController = new ServiceController("ScheduleService"))
                {
                    serviceController.Stop();
                }
            }
        }

        private void SchedularCallback(object e)
        {
            WriteToLog("Begin Backup Session: {0}");
            BackupController backupController = new BackupController();
            backupController.RunBackupAsync().ConfigureAwait(false);
        }

        private void WriteToLog(string message)
        {
            LogService.CreateLogAsync("New Daily Log Created").ConfigureAwait(false);
            LogService.LogEventAsync(message).ConfigureAwait(false);
        }
    }
}
