
using System;
using System.Collections;

namespace Schedule
{
    /// <summary>
    /// IScheduledItem represents a scheduled event. You can query it for the number of events
    /// that occur in a time interval and for the remaining interval before the next event.
    /// </summary>
    public interface IScheduledItem
	{
		/// <summary>
		/// Returns the times of the events that occur in the given time interval. The interval is closed
		/// at the start and open at the end so that intervals can be stacked without overlapping.
		/// </summary>
		/// <param name="Begin">The beginning of the interval</param>
		/// <param name="End">The end of the interval</param>
		/// <returns>All events >= Begin and &lt; End </returns>
		void AddEventsInInterval(DateTime Begin, DateTime End, ArrayList List);

		/// <summary>
		/// Returns the next run time of the scheduled item. Optionally excludes the starting time.
		/// </summary>
		/// <param name="time">The starting time of the interval</param>
		/// <param name="IncludeStartTime">if true then the starting time is included in the query false, it is excluded.</param>
		/// <returns>The next execution time either on or after the starting time.</returns>
		DateTime NextRunTime(DateTime time, bool IncludeStartTime);
	}
}
