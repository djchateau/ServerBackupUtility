
using System;

namespace Schedule
{
	/// <summary>Single event represents an event which only fires once.</summary>
	public class SingleEvent : IScheduledItem
	{
		public SingleEvent(DateTime eventTime)
		{
			_EventTime = eventTime;
		}

		public void AddEventsInInterval(DateTime Begin, DateTime End, System.Collections.ArrayList List)
		{
			if (Begin <= _EventTime && End > _EventTime)
				List.Add(_EventTime);
		}

		public DateTime NextRunTime(DateTime time, bool IncludeStartTime)
		{
			if (IncludeStartTime)
				return (_EventTime >= time) ? _EventTime : DateTime.MaxValue;
			else
				return (_EventTime >  time) ? _EventTime : DateTime.MaxValue;
		}

		private DateTime _EventTime;
	}
}
