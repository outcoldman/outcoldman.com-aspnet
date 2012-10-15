using System.IO;
using System.Text;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Core
{
	public class DebugTextWriter : TextWriter
	{
		public override void Write(char[] buffer, int index, int count)
		{
			if (Log4NetHelper.Log.IsDebugEnabled)
				Log4NetHelper.Log.Debug(new string(buffer, index, count));
		}

		public override void Write(string value)
		{
			if (Log4NetHelper.Log.IsDebugEnabled)
				Log4NetHelper.Log.Debug(value);
		}

		public override Encoding Encoding
		{
			get { return Encoding.Default; }
		}
	}
}