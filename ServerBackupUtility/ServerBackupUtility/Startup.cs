
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
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
            WriteToFile("Schedule Service started {0}");
            ScheduleService();
        }

        protected override void OnStop()
        {
            WriteToFile("Schedule Service stopped {0}");
            _schedular.Dispose();
        }

        public void ScheduleService()
        {
            try
            {
                _schedular = new Timer(new TimerCallback(SchedularCallback));

                switch (_mode)
                {
                    case "time":
                        if (DateTime.Now > _time)
                        {
                            // If Scheduled Time is passed, set schedule for the next day.
                            _time = _time.AddDays(1);
                        }

                        break;

                    case "interval":
                        // Set scheduled time by adding the interval to current time.
                        _time = DateTime.Now.AddMinutes(_interval);

                        if (DateTime.Now > _time)
                        {
                            // If scheduled time is passed, set schedule for next interval.
                            _time = _time.AddMinutes(_interval);
                        }

                        break;
                }

                TimeSpan timeSpan = _time.Subtract(DateTime.Now);
                string schedule = String.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                WriteToFile("Schedule Service scheduled to run after: " + schedule + " {0}");

                // Get the difference in minutes between the scheduled time and the current time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                // Change the timer's due time.
                _schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                WriteToFile("Schedule Service Error on: {0} " + ex.Message + ex.StackTrace);

                // Stop the Windows service.
                using (var serviceController = new System.ServiceProcess.ServiceController("ScheduleService"))
                {
                    serviceController.Stop();
                }
            }
        }

        private void SchedularCallback(object e)
        {
            WriteToFile("Schedule Service Log: {0}");
            ScheduleService();
        }

        private void WriteToFile(string text)
        {
            string path = "C:\\ServiceLog.txt";

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(String.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
            }
        }
    }
}
