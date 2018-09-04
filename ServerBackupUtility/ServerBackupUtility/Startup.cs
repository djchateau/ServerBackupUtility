
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
        private int _interval = Convert.ToInt16(ConfigurationManager.AppSettings["Interval"]);
        private readonly string _mode = ConfigurationManager.AppSettings["Mode"].ToLower();

        protected override void OnStart(string[] args)
        {
            WriteToFile("Schedule Service Started {0}");
            ScheduleService();
        }

        protected override void OnStop()
        {
            WriteToFile("Schedule Service Stopped {0}");
            _schedular.Dispose();
        }

        public void ScheduleService()
        {
            try
            {
                _schedular = new Timer(new TimerCallback(SchedularCallback));

                if (DateTime.Now > _time)
                {
                    // If scheduled time is passed, set schedule for the next day.
                    _time = _time.AddDays(1);
                }

                TimeSpan timeSpan = _time.Subtract(DateTime.Now);
                string schedule = String.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                WriteToFile("Schedule Service Scheduled to Run at: " + schedule + " {0}");

                // Get the difference in minutes between the scheduled time and the current time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                // Change the timer's due time.
                _schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                WriteToFile("Schedule Service Error on: {0} " + ex.Message);

                // Stop the Windows service.
                using (var serviceController = new ServiceController("ScheduleService"))
                {
                    serviceController.Stop();
                }
            }
        }

        private void SchedularCallback(object e)
        {
            WriteToFile("Begin Backup Session: {0}");
            BackupController backupController = new BackupController();
            backupController.RunBackupAsync().ConfigureAwait(false);
        }

        private void WriteToFile(string text)
        {
            LogService.LogEventAsync(text).ConfigureAwait(false);
        }
    }
}
