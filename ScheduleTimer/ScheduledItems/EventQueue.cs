
using System;
using System.Collections;

namespace Schedule
{
	/// <summary>
	/// The event queue is a collection of scheduled items that represents the union of all child scheduled items.
	/// This is useful for events that occur every 10 minutes or at multiple intervals not covered by the simple
	/// scheduled items.
	/// </summary>
	public class EventQueue : IScheduledItem
	{
		public EventQueue()
		{
			_List = new ArrayList();
		}

		/// <summary>
		/// Adds a ScheduledTime to the queue.
		/// </summary>
		/// <param name="time">The scheduled time to add</param>
		public void Add(IScheduledItem time)
		{
			_List.Add(time);
		}

		/// <summary>
		/// Clears the list of scheduled times.
		/// </summary>
		public void Clear()
		{
			_List.Clear();
		}

		/// <summary>
		/// Adds the running time for all events in the list.
		/// </summary>
		/// <param name="Begin">The beginning time of the interval</param>
		/// <param name="End">The end time of the interval</param>
		/// <param name="List">The list to add times to.</param>
		public void AddEventsInInterval(DateTime Begin, DateTime End, ArrayList List)
		{
            foreach (IScheduledItem st in _List)
            {
                st.AddEventsInInterval(Begin, End, List);
            }

			List.Sort();
		}

		/// <summary>
		/// Returns the first time after the starting time for all events in the list.
		/// </summary>
		/// <param name="time">The starting time.</param>
		/// <param name="AllowExact">If this is true then it allows the return time to match the time parameter, false forces the return time to be greater then the time parameter</param>
		/// <returns>Either the next event after the input time or greater or equal to depending on the AllowExact parameter.</returns>
		public DateTime NextRunTime(DateTime time, bool AllowExact)
		{
			DateTime next = DateTime.MaxValue;
			//Get minimum datetime from the list.

			foreach(IScheduledItem st in _List)
			{
				DateTime Proposed = st.NextRunTime(time, AllowExact);
				next = (Proposed < next) ? Proposed : next;
			}
			return next;
		}

		private ArrayList _List;
	}
}
