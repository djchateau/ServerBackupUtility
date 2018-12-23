
using System;
using System.Xml;

namespace Schedule
{
	/// <summary>
	/// Null event strorage disables error recovery by returning now for the last time an event fired.
	/// </summary>
	public class NullEventStorage : IEventStorage
	{
		public NullEventStorage()
		{

		}

		public void RecordLastTime(DateTime Time)
		{

		}

		public DateTime ReadLastTime()
		{
			return DateTime.Now;
		}
	}

	/// <summary>
	/// Local event strorage keeps the last time in memory so that skipped events are not recovered.
	/// </summary>
	public class LocalEventStorage : IEventStorage
	{
        private DateTime _lastTime;

        public LocalEventStorage()
		{
			_lastTime = DateTime.MaxValue;
		}

		public void RecordLastTime(DateTime Time)
		{
			_lastTime = Time;
		}

		public DateTime ReadLastTime()
		{
			if (_lastTime == DateTime.MaxValue)
            {
                _lastTime = DateTime.Now;
            }

            return _lastTime;
		}
	}

    /// <summary>
    /// FileEventStorage saves the last time in an XmlDocument so that
    /// recovery will include periods that the process is shutdown.
    /// </summary>
    public class FileEventStorage : IEventStorage
	{
        private readonly string _fileName;
        private readonly string _xPath;
        private readonly XmlDocument _doc = new XmlDocument();

        public FileEventStorage(string FileName, string XPath)
		{
			_fileName = FileName;
			_xPath = XPath;
		}

		public void RecordLastTime(DateTime Time)
		{
			_doc.SelectSingleNode(_xPath).Value = Time.ToString();
			_doc.Save(_fileName);
		}

		public DateTime ReadLastTime()
		{
			_doc.Load(_fileName);
			string Value = _doc.SelectSingleNode(_xPath).Value;

			if (String.IsNullOrEmpty(Value))
            {
                return DateTime.Now;
            }

            return DateTime.Parse(Value);
		}
	}
}
