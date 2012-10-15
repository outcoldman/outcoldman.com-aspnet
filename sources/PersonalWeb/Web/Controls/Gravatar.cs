using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace PersonalWeb.Web.Controls
{
	public static class Gravatar
	{
		public static string GetHash(this HtmlHelper helper, string email)
		{
			email = (email ?? string.Empty).ToLower();
			using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
			{
				byte[] bytesToHash = Encoding.ASCII.GetBytes(email);
				bytesToHash = md5.ComputeHash(bytesToHash);
				string result = "";
				foreach (byte b in bytesToHash)
				{
					result = (result + b.ToString("x2"));
				}
				return result;
			}
		}
	}
}