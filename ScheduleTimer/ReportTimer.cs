
using System;

namespace Schedule
{
	public class ReportEventArgs : EventArgs
	{
		public ReportEventArgs(DateTime time, int reportNumber)
        {
            EventTime = time;
            ReportNumber = reportNumber;
        }

		public int ReportNumber;
		public DateTime EventTime;
	}

	public delegate void ReportEventHandler(object sender, ReportEventArgs e);

	/// <summary>
	/// Summary description for ReportTimer.
	/// </summary>
	public class ReportTimer : ScheduleTimerBase
	{
		public void AddReportEvent(IScheduledItem schedule, int reportNumber)
		{
			if (Elapsed == null)
            {
                throw new Exception("You must set elapsed before adding Events");
            }

            AddJob(new TimerJob(schedule, new DelegateMethodCall(Handler, Elapsed, reportNumber)));
		}

		public void AddAsyncReportEvent(IScheduledItem schedule, int reportNumber)
		{
			if (Elapsed == null)
            {
                throw new Exception("You must set elapsed before adding Events");
            }

            TimerJob Event = new TimerJob(schedule, new DelegateMethodCall(Handler, Elapsed, reportNumber));
			Event.SyncronizedEvent = false;

			AddJob(Event);
		}

		public event EventHandler<ReportEventArgs> Elapsed;

		private delegate void ConvertHandler(ReportEventHandler Handler, int reportNumber, object sender, DateTime time);
		private static readonly ConvertHandler Handler = new ConvertHandler(Converter);

		static void Converter(ReportEventHandler Handler, int reportNumber, object sender, DateTime time)
		{
			if (Handler == null)
            {
                throw new ArgumentNullException(nameof(Handler));
            }

            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            Handler(sender, new ReportEventArgs(time, reportNumber));
		}
	}
}
