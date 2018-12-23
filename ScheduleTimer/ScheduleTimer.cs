
using System;
using System.Collections;
using System.Timers;

namespace Schedule
{
	/// <summary>
	/// <para>
	/// ScheduleTimer represents a timer that fires on a more human friendly schedule.  For example it is easy to 
	/// set it to fire every day at 6:00PM.  It is useful for batch jobs or alarms that might be difficult to 
	/// schedule with the native .net timers.
	/// </para>
	/// <para>
	/// It is similar to the .net timer that it is based on with the start and stop methods functioning similarly.
	/// The main difference is the event uses a different delegate and arguement since the .net timer argument 
	/// class is not creatable.
	/// </para>
	/// </summary>
	public class ScheduleTimerBase : IDisposable
	{
        /// <summary>
        /// This is here to enhance accuracy.  Even if nothing is scheduled the timer sleeps for a maximum of 1 minute.
        /// </summary>
        private static TimeSpan MaxInterval = new TimeSpan(0, 1, 0);

        private DateTime _lastTime;
        private readonly Timer _timer;
        private readonly TimerJobList _jobs;
        private volatile bool _stopFlag;

        public ScheduleTimerBase()
		{
			_timer = new Timer();
			_timer.AutoReset = false;
			_timer.Elapsed += Timer_Elapsed;
			_jobs = new TimerJobList();
			_lastTime = DateTime.MaxValue;
		}

        /// <summary>
        /// EventStorage determines the method used to store the last event fire time.  It defaults to keeping it in memory.
        /// </summary>
        public IEventStorage EventStorage = new LocalEventStorage();
        public event ExceptionEventHandler Error;

        /// <summary>
        /// Adds a job to the timer.  This method passes in a delegate and the parameters similar to the Invoke method of windows forms.
        /// </summary>
        /// <param name="Schedule">The schedule that this delegate is to be run on.</param>
        /// <param name="f">The delegate to run</param>
        /// <param name="Params">The method parameters to pass if you leave any DateTime parameters unbound, then they will be set with the scheduled run time of the 
        /// method.  Any unbound object parameters will get this Job object passed in.</param>
        public void AddJob(IScheduledItem Schedule, Delegate f, params object[] Params)
		{
			_jobs.Add(new TimerJob(Schedule, new DelegateMethodCall(f, Params)));
		}

		/// <summary>
		/// Adds a job to the timer to operate asyncronously.
		/// </summary>
		/// <param name="Schedule">The schedule that this delegate is to be run on.</param>
		/// <param name="f">The delegate to run</param>
		/// <param name="Params">The method parameters to pass if you leave any DateTime parameters unbound, then they will be set with the scheduled run time of the 
		/// method.  Any unbound object parameters will get this Job object passed in.</param>
		public void AddAsyncJob(IScheduledItem Schedule, Delegate f, params object[] Params)
		{
			TimerJob Event = new TimerJob(Schedule, new DelegateMethodCall(f, Params));

			Event.SyncronizedEvent = false;
			_jobs.Add(Event);
		}

		/// <summary>
		/// Adds a job to the timer.  
		/// </summary>
		/// <param name="Event"></param>
		public void AddJob(TimerJob Event)
		{
			_jobs.Add(Event);
		}

		/// <summary>
		/// Clears out all scheduled jobs.
		/// </summary>
		public void ClearJobs()
		{
			_jobs.Clear();
		}

		/// <summary>
		/// Begins executing all assigned jobs at the scheduled times
		/// </summary>
		public void Start()
		{
			_stopFlag = false;
			QueueNextTime(EventStorage.ReadLastTime());
		}

		/// <summary>
		/// Halts executing all jobs.  When the timer is restarted all jobs that would have run while the timer was stopped are re-tried.
		/// </summary>
		public void Stop()
		{
			_stopFlag = true;
			_timer.Stop();
		}

		private double NextInterval(DateTime thisTime)
		{
			TimeSpan interval = _jobs.NextRunTime(thisTime)-thisTime;

			if (interval > MaxInterval)
            {
                interval = MaxInterval;
            }

            // Handles the case of 0 wait time, the interval property requires a duration > 0.
            return (interval.TotalMilliseconds == 0) ? 1 : interval.TotalMilliseconds;
		}

		private void QueueNextTime(DateTime thisTime)
		{
			_timer.Interval = NextInterval(thisTime);
			System.Diagnostics.Debug.WriteLine(_timer.Interval);
			_lastTime = thisTime;
			EventStorage.RecordLastTime(thisTime);
			_timer.Start();
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				if (_jobs == null)
                {
                    return;
                }

                _timer.Stop();

				foreach(TimerJob Event in _jobs.Jobs)
				{
					try
                    {
                        Event.Execute(this, _lastTime, e.SignalTime, Error);
                    }
					catch (Exception ex)
                    {
                        OnError(DateTime.Now, Event, ex);
                    }
				}
			}
			catch (Exception ex)
			{
				OnError(DateTime.Now, null, ex);
			}
			finally
			{
				if (_stopFlag == false)
                {
                    QueueNextTime(e.SignalTime);
                }
            }
		}

		private void OnError(DateTime eventTime, TimerJob job, Exception e)
		{
			if (Error == null)
            {
                return;
            }

            try
            {
                Error(this, new ExceptionEventArgs(eventTime, e));
            }
			catch (Exception)
            {

            }
		}

        private bool disposed = false;

		public void Dispose()
		{
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                }
            }

            disposed = true;
        }
	}

	public class ScheduleTimer : ScheduleTimerBase
	{
        /// <summary>
        /// The event to fire when you only need to fire one event.
        /// </summary>
        public event ScheduledEventHandler Elapsed;

        /// <summary>
        /// Add event is used in conjunction with the Elaspsed event handler. Set the Elapsed handler, add your schedule and call start.
        /// </summary>
        /// <param name="Schedule">The schedule to fire the event at. Adding additional schedules will cause the event to fire whenever either schedule calls for it.</param>
        public void AddEvent(IScheduledItem Schedule)
		{
			if (Elapsed == null)
            {
                throw new ArgumentNullException("Elapsed", "Member Variable is Null");
            }

            AddJob(new TimerJob(Schedule, new DelegateMethodCall(Elapsed)));
		}
	}

	/// <summary>
	/// ExceptionEventArgs allows exceptions to be captured and sent to the OnError event of the timer.
	/// </summary>
	public class ExceptionEventArgs : EventArgs
	{
        public DateTime EventTime;
        public Exception Error;

        public ExceptionEventArgs(DateTime eventTime, Exception e)
		{
			EventTime = eventTime;
			Error = e;
		}
	}

	/// <summary>
	/// ExceptionEventHandler is the method type used by the OnError event for the timer.
	/// </summary>
	public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs Args);

	public class ScheduledEventArgs : EventArgs
	{
        public DateTime EventTime;

        public ScheduledEventArgs(DateTime eventTime)
		{
			EventTime = eventTime;
		}
	}

	public delegate void ScheduledEventHandler(object sender, ScheduledEventArgs e);

	/// <summary>
	/// The IResultFilter interface represents filters that either sort the events for an interval or
	/// remove duplicate events either selecting the first or the last event.
	/// </summary>
	public interface IResultFilter
	{
		void FilterResultsInInterval(DateTime start, DateTime end, ArrayList list);
	}

	/// <summary>
	/// IEventStorage is used to provide persistance of schedule during service shutdowns.
	/// </summary>
	public interface IEventStorage
	{
		void RecordLastTime(DateTime Time);
		DateTime ReadLastTime();
	}
}
