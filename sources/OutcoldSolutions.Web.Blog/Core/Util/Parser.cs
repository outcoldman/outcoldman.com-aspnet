using System;
using System.Globalization;

namespace PersonalWeb.Core.Util
{
    using System.Diagnostics;

    public static class Parser
	{
		/// <summary>
		/// 	Try cast <paramref name = "obj" /> value to type <typeparamref name = "T" />,
		/// 	if can't will return default(<typeparamref name = "T" />)
		/// </summary>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "obj"></param>
		/// <returns></returns>
		public static T To<T>(this object obj)
		{
			return To(obj, default(T));
		}

		/// <summary>
		/// 	Try cast <paramref name = "obj" /> value to type <typeparamref name = "T" />,
		/// 	if can't will return <paramref name = "defaultValue" />
		/// </summary>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "obj"></param>
		/// <param name = "defaultValue"></param>
		/// <returns></returns>
		public static T To<T>(this object obj, T defaultValue)
		{
			if (obj == null)
				return defaultValue;

			if (obj is T)
				return (T) obj;

			Type type = typeof (T);

			if (type == typeof(string))
			{
				return (T) (object) obj.ToString();
			}

			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				return To(obj, defaultValue, underlyingType);
			}

			return To(obj, defaultValue, type);
		}

		private static T To<T>(object obj, T defaultValue, Type type)
		{
			if (type.IsEnum)
			{
				if (Enum.IsDefined(type, obj)) return (T) Enum.Parse(type, obj.ToString());
				return defaultValue;
			}

			try
			{
				return (T) Convert.ChangeType(obj, type);
			}
			catch (Exception e)
			{
                Trace.TraceError(e.ToString());
				return defaultValue;
			}
		}

    public static DateTime ToDateTime(object  obj, string format)
    {
      if (obj == null)
        return default(DateTime);
      try
      {
        IFormatProvider culture = new CultureInfo("en-US");
        return DateTime.ParseExact(obj.ToString(), format, culture);
      }
      catch (Exception e)
      {
          Trace.TraceError(e.ToString());
        return default(DateTime);
      }
     
    }
	}
}