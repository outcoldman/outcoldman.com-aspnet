namespace OutcoldSolutions.Web.Blog.Core.Akismet
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Web;

    /// <summary>
	/// 	Manager to check items for spam and submit unrecognized ones
	/// </summary>
	public class AkismetManager
	{
		private string _AppVersion;

		/// <summary>
		/// 	Gets the application version which will be sent as the user agent
		/// </summary>
		protected string AppVersion
		{
			get
			{
				if (this._AppVersion == null)
				{
					try
					{
						this._AppVersion = Assembly.GetExecutingAssembly().GetName().Version.Major + "." +
						              Assembly.GetExecutingAssembly().GetName().Version.Minor;
					}
					catch
					{
						this._AppVersion = "0.0";
					}
				}

				return this._AppVersion;
			}
		}

		private string _AppName;

		/// <summary>
		/// 	Gets the application name which will be sent as the user agent
		/// </summary>
		protected string AppName
		{
			get
			{
				if (this._AppName == null)
				{
					try
					{
						this._AppName = Assembly.GetExecutingAssembly().GetName().Name;
					}
					catch
					{
						this._AppName = "Unknown-C#-User";
					}
				}

				return this._AppName;
			}
		}

		private string _ApiKey;

		/// <summary>
		/// 	Sets or gets your API key for Akismet. Get one at http://wordpress.com/api-keys/
		/// </summary>
		/// <exception cref = "ArgumentNullException">ApiKey is null</exception>
		/// <exception cref = "ArgumentOutOfRangeException">ApiKey may not be empty</exception>
		public string ApiKey
		{
			get { return this._ApiKey; }
			set
			{
				if (value == null) throw new ArgumentNullException("ApiKey");
				if (value.Length <= 0) throw new ArgumentOutOfRangeException("ApiKey", value, "ApiKey may not be empty");
				this._ApiKey = value;
			}
		}

		private string _Url;

		/// <summary>
		/// 	Gets or sets the URL of your homepage/blog/wiki whatever
		/// </summary>
		/// <exception cref = "ArgumentNullException">Url is null</exception>
		/// <exception cref = "ArgumentOutOfRangeException">Url may not be empty</exception>
		public string Url
		{
			get { return this._Url; }
			set
			{
				if (value == null) throw new ArgumentNullException("Url");
				if (value.Length <= 0) throw new ArgumentOutOfRangeException("Url", value, "Url may not be empty");
				this._Url = value;
			}
		}

		private int _TimeOut = 10000;

		/// <summary>
		/// 	Gets or sets the timeout for Akismet requests. Note that Akismet may need longer if you specify less paramters
		/// </summary>
		/// <exception cref = "ArgumentOutOfRangeException">TimeOut must be greater than zero</exception>
		public int TimeOut
		{
			get { return this._TimeOut; }
			set
			{
				if (value <= 0) throw new ArgumentOutOfRangeException("TimeOut", value, "TimeOut must be greater than zero");
				this._TimeOut = value;
			}
		}

		/// <summary>
		/// 	Executes a HTTP request to a specified URL and returns the response content
		/// </summary>
		/// <param name = "url">The URL</param>
		/// <param name = "content">The request content</param>
		/// <returns>The response content</returns>
		private string ExecuteRequest(string url, string content)
		{
			HttpWebRequest request = this.PrepareRequest(url);
			request.ContentLength = content.Length;

			if (content != null && content.Length > 0)
			{
				using (Stream contentStream = request.GetRequestStream())
				{
					using (StreamWriter contentWriter = new StreamWriter(contentStream))
					{
						contentWriter.Write(content);
					}
				}
			}

			string response = null;

			using (Stream responseStream = request.GetResponse().GetResponseStream())
			{
				using (StreamReader responseReader = new StreamReader(responseStream))
				{
					response = responseReader.ReadToEnd();
				}
			}

			return response;
		}

		/// <summary>
		/// 	Returns a prepared HttpWebRequest object
		/// </summary>
		/// <param name = "url">The URL to request</param>
		/// <returns>HttpWebRequest object</returns>
		private HttpWebRequest PrepareRequest(string url)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
			request.UserAgent = this.AppName + "/" + this.AppVersion;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			request.Timeout = this._TimeOut;
			return request;
		}

		/// <summary>
		/// 	Checks if your Akismet API key is correct
		/// </summary>
		/// <returns>True on success</returns>
		public bool IsValidKey()
		{
			string content = "key=" + HttpUtility.UrlEncode(this._ApiKey) + "&blog=" + HttpUtility.UrlEncode(this._Url);

			string response = this.ExecuteRequest("http://rest.akismet.com/1.1/verify-key", content);

			if (response != null && response.ToLower() == "valid")
			{
				return true;
			}
			else return false;
		}

		/// <summary>
		/// 	Checks if the given item is spam or not
		/// </summary>
		/// <param name = "item">The item to check</param>
		/// <returns>true on spam, false if not</returns>
		/// <exception cref = "ArgumentNullException">item is null</exception>
		/// <exception cref = "InvalidKeyException">Invalid API key</exception>
		/// <exception cref = "AkismetException">Invalid server reply</exception>
		/// <exception cref = "AkismetException">Empty server reply</exception>
		public bool IsSpam(AkismetItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			string content = this.AddDefaultFields(item.ToString());
			string response = this.ExecuteRequest("http://" + this._ApiKey + ".rest.akismet.com/1.1/comment-check", content);

			if (response != null)
			{
				if (response.ToLower() == "true") return true;
				else if (response.ToLower() == "false") return false;
				else if (response.ToLower() == "invalid") throw new InvalidKeyException();
				else throw new AkismetException("Invalid server reply");
			}
			else throw new AkismetException("Empty server reply");
		}

		/// <summary>
		/// 	Submits an item as spam
		/// </summary>
		/// <param name = "item">The item to submit</param>
		/// <exception cref = "ArgumentNullException">item is null</exception>
		/// <exception cref = "InvalidKeyException">Invalid API key</exception>
		/// <exception cref = "AkismetException">Invalid server reply</exception>
		public void SubmitSpam(AkismetItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			string content = this.AddDefaultFields(item.ToString());
			string response = this.ExecuteRequest("http://" + this._ApiKey + ".rest.akismet.com/1.1/submit-spam", content);

			if (response.ToLower() == "invalid") throw new InvalidKeyException();
		}

		/// <summary>
		/// 	Submits an item as ham (not spam)
		/// </summary>
		/// <param name = "item">The item to submit</param>
		/// <exception cref = "ArgumentNullException">item is null</exception>
		public void SubmitHam(AkismetItem item)
		{
			if (item == null) throw new ArgumentNullException("item");

			string content = "key=" + HttpUtility.UrlEncode(this._ApiKey) + "&blog=" + HttpUtility.UrlEncode(this._Url) + item;
			string response = this.ExecuteRequest("http://" + this._ApiKey + ".rest.akismet.com/1.1/submit-ham", content);

			if (response.ToLower() == "invalid") throw new InvalidKeyException();
		}

		private string AddDefaultFields(string content)
		{
			return "key=" + HttpUtility.UrlEncode(this._ApiKey) + "&blog=" + HttpUtility.UrlEncode(this._Url) + content;
		}

		/// <summary>
		/// 	Creates a new Akismet manager
		/// </summary>
		/// <param name = "apiKey">Your API key for Akismet. Get one at http://wordpress.com/api-keys/</param>
		/// <param name = "url">The URL of your homepage/blog/wiki whatever</param>
		/// <exception cref = "ArgumentNullException">apiKey is null</exception>
		/// <exception cref = "ArgumentOutOfRangeException">apiKey is empty</exception>
		/// <exception cref = "ArgumentNullException">url is null</exception>
		/// <exception cref = "ArgumentOutOfRangeException">url is empty</exception>
		public AkismetManager(string apiKey, string url)
		{
			if (apiKey == null) throw new ArgumentNullException("apiKey");
			if (apiKey.Length <= 0) throw new ArgumentOutOfRangeException("apiKey", apiKey, "apiKey may not be empty");

			if (url == null) throw new ArgumentNullException("url");
			if (url.Length <= 0) throw new ArgumentOutOfRangeException("url", url, "url may not be empty");

			this._ApiKey = apiKey;
			this._Url = url;
		}
	}
}