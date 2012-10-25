// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.Web.Blog.Core.Util
{
    using System;
    using System.Configuration;
    using System.Diagnostics;

    public static class ConfigurationUtil
    {
        public static int DefaultCacheValue
        {
            get
            {
                return GetSettings("DefaultCacheValue", 60);
            }
        }

        public static string AuthorUsername
        {
            get
            {
                return GetSettings("AuthorUsername", "admin");
            }
        }

        public static string AuthorEmail
        {
            get
            {
                return GetSettings("AuthorEmail", "admin@example.com");
            }
        }

        public static string SiteUrl
        {
            get
            {
                return GetSettings("SiteUrl", "http://example.com");
            }
        }

        private static ConnectionStringSettingsCollection ConnectionStrings
        {
            get
            {
                return ConfigurationManager.ConnectionStrings;
            }
        }

        public static ConnectionStringSettings GetConnectionString(string connectionStringName)
        {
            ConnectionStringSettings settings = ConnectionStrings[connectionStringName];
            if (settings == null)
            {
                throw new InvalidOperationException(
                    string.Format("Connection string '{0}' not found!", connectionStringName));
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
                string message =
                    string.Format(
                        "Failed to parse configured value '{1}' for {0}. Using {2} as default value.", 
                        name, 
                        value, 
                        defaultValue);
                Trace.TraceError(ex.ToString());
                Trace.TraceError(message);

                return defaultValue;
            }
        }
    }
}