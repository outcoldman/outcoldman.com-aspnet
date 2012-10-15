using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace PersonalWeb.Core.Util
{
    public class Log4NetHelper
    {
    	private static ILog _logger;

    	public static ILog Log
    	{
			get
			{
				if (_logger == null)
					InitDebugLogger();
				return _logger;
			}
			private set { _logger = value; }
    	}

        private const string Key = "Log4NetConfiguration";

        public static void InitLogger()
        {
			string pathFileConfiguration = System.Configuration.ConfigurationManager.AppSettings.Get(Key);
			XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + pathFileConfiguration));
			Log = LogManager.GetLogger("Application");
        }

		private static void InitDebugLogger()
		{
			TraceAppender traceAppender = new TraceAppender();

			PatternLayout patternLayout = new PatternLayout
			{
				ConversionPattern =
					"%date{yyyy-MM-dd HH:mm:ss} [%thread] %-5level %logger{2} - %message%newline"
			};
			patternLayout.ActivateOptions();

			traceAppender.Layout = patternLayout;

			BasicConfigurator.Configure(traceAppender);
			Log = LogManager.GetLogger("Application");
		}
    }
}
