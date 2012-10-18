using System.Web.Mvc;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Web
{
	public class CustomOutputCacheAttribute : OutputCacheAttribute
	{
		public CustomOutputCacheAttribute()
		{
			Duration = ConfigurationUtil.DefaultCacheValue;
			VaryByParam = "none";
		}
	}
}
