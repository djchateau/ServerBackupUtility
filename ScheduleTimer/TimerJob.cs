
using System;
using System.Collections;
using System.Reflection;

namespace Schedule
{
	/// <summary>
	/// Timer job groups a schedule, syncronization data, a result filter, method information and an enabled state so that multiple jobs
	/// can be managed by the same timer, each one operating independently of the others with different syncronization and recovery settings.
	/// </summary>
	public class TimerJob
	{
        private readonly ExecuteHandler _executeHandler;
        private delegate void ExecuteHandler(object sender, DateTime EventTime, ExceptionEventHandler Error);

        public TimerJob(IScheduledItem schedule, IMethodCall method)
		{
			Schedule = schedule;
			Method = method;
			_executeHandler = new ExecuteHandler(ExecuteInternal);
		}

		public IScheduledItem Schedule;
		public bool SyncronizedEvent = true;
		public IResultFilter Filter;
		public IMethodCall Method;
        // public IJobLog Log;
		public bool Enabled = true;

		public DateTime NextRunTime(DateTime time, bool IncludeStartTime)
		{
			if (!Enabled)
            {
                return DateTime.MaxValue;
            }

            return Schedule.NextRunTime(time, IncludeStartTime);
		}

		public void Execute(object sender, DateTime Begin, DateTime End, ExceptionEventHandler Error)
		{
			if (!Enabled)
            {
                return;
            }

            ArrayList EventList = new ArrayList();
			Schedule.AddEventsInInterval(Begin, End, EventList);

			if (Filter != null)
            {
                Filter.FilterResultsInInterval(Begin, End, EventList);
            }

            foreach (DateTime EventTime in EventList)
			{
				if (SyncronizedEvent)
                {
                    _executeHandler(sender, EventTime, Error);
                }
                else
                {
                    _executeHandler.BeginInvoke(sender, EventTime, Error, null, null);
                }
            }
		}

		private void ExecuteInternal(object sender, DateTime EventTime, ExceptionEventHandler Error)
		{
			try 
			{
				TimerParameterSetter Setter = new TimerParameterSetter(EventTime, sender);
				Method.Execute(Setter);
			}
			catch (Exception ex)
			{
				if (Error != null)
                {
                    try
                    {
                        Error(this, new ExceptionEventArgs(EventTime, ex));
                    }
                    catch
                    {

                    }
                }
            }
		}
	}

	/// <summary>
	/// Timer job manages a group of timer jobs.
	/// </summary>
	public class TimerJobList
	{
        private readonly ArrayList _list;

        public TimerJobList()
		{
			_list = new ArrayList();
		}

		public void Add(TimerJob Event)
		{
			_list.Add(Event);
		}

		public void Clear()
		{
			_list.Clear();
		}

		/// <summary>
		/// Gets the next time any of the jobs in the list will run.  Allows matching the exact start time.  If no matches are found the return
		/// is DateTime.MaxValue;
		/// </summary>
		/// <param name="time">The starting time for the interval being queried.  This time is included in the interval</param>
		/// <returns>The first absolute date one of the jobs will execute on.  If none of the jobs needs to run DateTime.MaxValue is returned.</returns>
		public DateTime NextRunTime(DateTime time)
		{
			DateTime next = DateTime.MaxValue;

			// Get minimum datetime from the list.
			foreach(TimerJob Job in _list)
			{
				DateTime Proposed = Job.NextRunTime(time, true);
				next = (Proposed < next) ? Proposed : next;
			}

			return next;
		}

		public TimerJob[] Jobs
		{
			get { return (TimerJob[])_list.ToArray(typeof(TimerJob)); }
		}
	}

	/// <summary>
	/// The timer job allows delegates to be specified with unbound parameters.  This ParameterSetter assigns all unbound datetime parameters
	/// with the specified time and all unbound object parameters with the calling object.
	/// </summary>
	public class TimerParameterSetter : IParameterSetter
	{
        private readonly DateTime _time;
        private readonly object _sender;

        /// <summary>
        /// Initalize the ParameterSetter with the time to pass to unbound time parameters and object to pass to unbound object parameters.
        /// </summary>
        /// <param name="time">The time to pass to the unbound DateTime parameters</param>
        /// <param name="sender">The object to pass to the unbound object parameters</param>
        public TimerParameterSetter(DateTime time, object sender)
		{
			_time = time;
			_sender = sender;
		}

		public void Reset()
		{

		}

		public bool GetParameterValue(ParameterInfo parameterInfo, int parameterLocation, ref object parameter)
		{
			switch(parameterInfo.ParameterType.Name.ToLower())
			{
				case "datetime":
					parameter = _time;
					return true;
				case "object":
					parameter = _sender;
					return true;
				case "scheduledeventargs":
					parameter = new ScheduledEventArgs(_time);
					return true;
				case "eventargs":
					parameter = new ScheduledEventArgs(_time);
					return true;
			}

			return false;
		}
	}
}
