
using System;
using System.Collections;

namespace Schedule
{
	/// <summary>
	/// This is an empty filter that does not filter any of the events.
	/// </summary>
	public class Filter : IResultFilter
	{
		public static IResultFilter Empty = new Filter();
		private Filter() {}

		public void FilterResultsInInterval(DateTime start, DateTime end, ArrayList list)
		{
			if (list == null)
            {
                return;
            }

            list.Sort();
		}
	}

	/// <summary>
	/// This causes only the first event of the interval to be counted.
	/// </summary>
	public class FirstEventFilter : IResultFilter
	{
		public static IResultFilter Filter = new FirstEventFilter();
		private FirstEventFilter()
        {

        }

		public void FilterResultsInInterval(DateTime start, DateTime end, ArrayList list)
		{
            if (list == null)
            {
                return;
            }

            if (list.Count < 2)
            {
                return;
            }

			list.Sort();
			list.RemoveRange(1, list.Count-1);
		}
	}

	/// <summary>
	/// This causes only the last event of the interval to be counted.
	/// </summary>
	public class LastEventFilter : IResultFilter
	{
		public static IResultFilter Filter = new LastEventFilter();
		private LastEventFilter() {}

		public void FilterResultsInInterval(DateTime start, DateTime end, ArrayList list)
		{
			if (list == null)
            {
                return;
            }

            if (list.Count < 2)
            {
                return;
            }

            list.Sort();
			list.RemoveRange(0, list.Count-1);
		}
	}
}
