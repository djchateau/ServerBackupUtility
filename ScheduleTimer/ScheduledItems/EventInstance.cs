
using System;

namespace Schedule
{
	/// <summary>
	/// There have been quite a few requests to allow scheduling of multiple delegates and method parameter data
	/// from the same timer.  This class allows you to match the event with the time that it fired.  I want to keep
	/// the same simple implementation of the EventQueue and interval classes since they can be reused elsewhere.
	/// The timer should be responsible for matching this data up.
	/// </summary>
	public class EventInstance : IComparable
	{
		public EventInstance(DateTime time, IScheduledItem scheduleItem, object data)
		{
			Time = time;
			ScheduleItem = scheduleItem;
			Data = data;
		}

		public DateTime Time;
		public IScheduledItem ScheduleItem;
		public object Data;

		public int CompareTo(object obj)
		{
			if (obj is EventInstance)
				return Time.CompareTo(((EventInstance)obj).Time);
			if (obj is DateTime)
				return Time.CompareTo((DateTime)obj);
			return 0;
		}
	}
}
