﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace PersonalWeb.Core.Util
{
    using System;
    using System.Configuration;
    using System.Reflection;

    using log4net;

    public static class ConfigurationUtil
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ConnectionStringSettings ConnectionStringMain
        {
            get { return GetConnectionString(MainConnectionString); }
        }

        public static int DefaultCacheValue
        {
            get { return GetSettings("DefaultCacheValue", 60); }
        }

        public static string Me
        {
            get { return GetSettings("MeUsername", "outcoldman"); }
        }

        public static string MeEmail
        {
            get { return GetSettings("MeEmail", "outcoldman@gmail.com"); }
        }

        public static string SiteUrl
        {
            get { return GetSettings("SiteUrl", "http://outcoldman.ru"); }
        }

        public static string LivejournalEmail
        {
            get { return GetSettings("LivejournalEmail", "outcoldman+outcoldan@post.livejournal.com"); }
        }

        public static string MainConnectionString
        {
            get { return GetSettings("MainConnectionString", "Main"); }
        }

        private static ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return ConfigurationManager.ConnectionStrings; }
        }

        public static ConnectionStringSettings GetConnectionString(string connectionStringName)
        {
            ConnectionStringSettings settings = ConnectionStrings[connectionStringName];
            if (settings == null)
            {
                throw new InvalidOperationException(string.Format("Connection string '{0}' not found!", connectionStringName));
            }

            return settings;
        }

        public static string GetSettings(string name, string defaultValue)
        {
            string result = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(result))
            {
                return defaultValue;
            }

            return result;
        }

        public static T GetSettings<T>(string name, T defaultValue)
        {
            string result = GetSettings(name, null);
            return result.To(defaultValue);
        }

        public static T GetEnumSettings<T>(string name, T defaultValue)
        {
            string value = GetSettings(name, null);

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch (Exception ex)
            {
                if (Log.IsErrorEnabled)
                {
                    string message = string.Format("Failed to parse configured value '{1}' for {0}. Using {2} as default value.", name, value, defaultValue);
                    Log.Error(message, ex);
                }
                return defaultValue;
            }
        }
    }
}