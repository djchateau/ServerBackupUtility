
using ServerBackupUtility.Logging;
using ServerBackupUtility.Services;
using System;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace ServerBackupUtility
{
    public partial class Startup : ServiceBase
    {
        private Timer _scheduler = null;
        private IRestartService _restartService = null;
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private string _mode = ConfigurationManager.AppSettings["SchedulerMode"].ToLower().Trim();
        private DateTime _time = DateTime.Parse(ConfigurationManager.AppSettings["ClockTime"].Trim());
        private int _minutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalTime"].Trim());

        public Startup()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToLog("Backup Scheduler Service Started");

            _restartService = new RestartService();
            _restartService.WatchAppConfig();

            if (args.Any() && args[0] == "debug")
            {
                _mode = "interval";
                _minutes = 1;
            }

            SchedulerService();
        }

        protected override void OnStop()
        {
            WriteToLog("Backup Scheduler Service Stopped");
            _scheduler.Dispose();
        }

        public void SchedulerService()
        {
            try
            {
                _scheduler = new Timer(new TimerCallback(SchedulerCallback));

                switch (_mode)
                {
                    case "clock":
                        if (_time > DateTime.Parse("23:30") && _time < DateTime.Parse("00:00"))
                        {
                            _time = DateTime.Parse("00:00");
                        }

                        if (DateTime.Now > _time)
                        {
                            // If scheduled time is passed, set schedule for the next day.
                            _time = _time.AddDays(1);
                        }

                        break;

                    case "interval":
                        _time = DateTime.Now.AddMinutes(_minutes);

                        if (DateTime.Now > _time)
                        {
                            _time = _time.AddMinutes(_minutes);
                        }

                        break;
                }

                TimeSpan timeSpan = _time.Subtract(DateTime.Now);
                string schedule = String.Format("{0} days {1} hours {2} minutes {3} seconds", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                WriteToLog(String.Format("Backup Scheduler Service Scheduled to Run in: {0}", schedule));

                // Get the difference in minutes between the scheduled time and the current time.
                int minutes = Convert.ToInt32(timeSpan.TotalMilliseconds);

                // Change the timer's due time.
                _scheduler.Change(minutes, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                WriteToLog(String.Format("Backup Scheduler Service Error: {0}", ex.Message));

                // Stop the Windows service.
                using (ServiceController serviceController = new ServiceController("BackupScheduler"))
                {
                    serviceController.Stop();
                }
            }
        }

        private void SchedulerCallback(object e)
        {
            WriteToLog("Begin Backup Session");
            ServicesController servicesController = new ServicesController();
            servicesController.RunBackup();
        }

        private void WriteToLog(string message)
        {
            LogService.CreateLog("Daily Log Created");
            LogService.LogEvent(message);
        }
    }
}
