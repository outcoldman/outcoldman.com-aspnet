using System;
using System.Net;
using System.Web;

namespace PersonalWeb.Core.Akismet
{
  /// <summary>
  ///   Represents an Akismet item like a comment or registration data
  /// </summary>
  public class AkismetItem
  {
    private string _IpAddress;

    /// <summary>
    ///   IP address of the comment submitter.
    /// </summary>
    /// <exception cref = "ArgumentNullException">IpAddress is null</exception>
    /// <exception cref = "ArgumentOutOfRangeException">IpAddress may not be empty</exception>
    public string IpAddress
    {
      get { return _IpAddress; }
      set
      {
        if (value == null) throw new ArgumentNullException("IpAddress");
        if (value.Length <= 0) throw new ArgumentOutOfRangeException("IpAddress", value, "IpAddress may not be empty");

        _IpAddress = value;
      }
    }

    private string _UserAgent;

    /// <summary>
    ///   User agent information. This value is required!
    /// </summary>
    /// <exception cref = "ArgumentNullException">UserAgent is null</exception>
    /// <exception cref = "ArgumentOutOfRangeException">UserAgent may not be empty</exception>
    public string UserAgent
    {
      get { return _UserAgent; }
      set
      {
        if (value == null) throw new ArgumentNullException("UserAgent");
        if (value.Length <= 0) throw new ArgumentOutOfRangeException("UserAgent", value, "UserAgent may not be empty");
        _UserAgent = value;
      }
    }

    private string _Referer;

    /// <summary>
    ///   The content of the HTTP_REFERER header should be sent here.
    /// </summary>
    public string Referer
    {
      get { return _Referer; }
      set { _Referer = value; }
    }

    private string _PermaLink;

    /// <summary>
    ///   The permanent location of the entry the comment was submitted to.
    /// </summary>
    public string PermaLink
    {
      get { return _PermaLink; }
      set { _PermaLink = value; }
    }

    private AkismetType _Type = AkismetType.General;

    /// <summary>
    ///   The type of this comment
    /// </summary>
    public AkismetType Type
    {
      get { return _Type; }
      set { _Type = value; }
    }

    private string _AuthorName;

    /// <summary>
    ///   Submitted name with the comment
    /// </summary>
    public string AuthorName
    {
      get { return _AuthorName; }
      set { _AuthorName = value; }
    }

    private string _AuthorEmail;

    /// <summary>
    ///   Submitted email address
    /// </summary>
    public string AuthorEmail
    {
      get { return _AuthorEmail; }
      set { _AuthorEmail = value; }
    }

    private string _AuthorUrl;

    /// <summary>
    ///   Commenter URL
    /// </summary>
    public string AuthorUrl
    {
      get { return _AuthorUrl; }
      set { _AuthorUrl = value; }
    }

    private string _Content;

    /// <summary>
    ///   The comment content
    /// </summary>
    public string Content
    {
      get { return _Content; }
      set { _Content = value; }
    }

    /// <summary>
    ///   Creates a new item
    /// </summary>
    /// <param name = "ipAddress">IP address of the comment submitter.</param>
    /// <param name = "userAgent">User agent information.</param>
    /// <exception cref = "ArgumentNullException">ipAddress is null</exception>
    /// <exception cref = "ArgumentNullException">userAgent is null</exception>
    /// <exception cref = "ArgumentOutOfRangeException">ipAddress may not be empty</exception>
    /// <exception cref = "ArgumentOutOfRangeException">userAgent may not be empty</exception>
    public AkismetItem(string ipAddress, string userAgent)
    {
      if (ipAddress == null) throw new ArgumentNullException("ipAddress");
      if (userAgent == null) throw new ArgumentNullException("userAgent");

      if (ipAddress.Length <= 0)
        throw new ArgumentOutOfRangeException("ipAddress", ipAddress, "ipAddress may not be empty");
      if (userAgent.Length <= 0)
        throw new ArgumentOutOfRangeException("userAgent", userAgent, "userAgent may not be empty");

      _IpAddress = ipAddress;
      _UserAgent = userAgent;
    }

    /// <summary>
    ///   Creates a new item
    /// </summary>
    /// <param name = "ipAddress">IP address of the comment submitter.</param>
    /// <param name = "userAgent">User agent information.</param>
    /// <exception cref = "ArgumentNullException">ipAddress is null</exception>
    /// <exception cref = "ArgumentNullException">userAgent is null</exception>
    /// <exception cref = "ArgumentOutOfRangeException">userAgent may not be empty</exception>
    public AkismetItem(IPAddress ipAddress, string userAgent)
    {
      if (ipAddress == null) throw new ArgumentNullException("ipAddress");
      if (userAgent == null) throw new ArgumentNullException("userAgent");

      if (userAgent.Length <= 0)
        throw new ArgumentOutOfRangeException("userAgent", userAgent, "userAgent may not be empty");

      _IpAddress = ipAddress.ToString();
      _UserAgent = userAgent;
    }

    /// <summary>
    ///   Returns an UrlEncoded string of all parameters
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      string encoded = "&user_agent=" + HttpUtility.UrlEncode(_UserAgent)
                       + "&user_ip=" + HttpUtility.UrlEncode(_IpAddress)
                       + (_Referer != null && _Referer.Length > 0 ? "&referrer=" + HttpUtility.UrlEncode(_Referer) : "")
                       +
                       (_PermaLink != null && _PermaLink.Length > 0
                          ? "&permalink=" + HttpUtility.UrlEncode(_PermaLink)
                          : "")
                       +
                       (_Type != AkismetType.General ? "&comment_type=" + HttpUtility.UrlEncode(_Type.ToString()) : "")
                       +
                       (_AuthorName != null && _AuthorName.Length > 0
                          ? "&comment_author=" + HttpUtility.UrlEncode(_AuthorName)
                          : "")
                       +
                       (_AuthorEmail != null && _AuthorEmail.Length > 0
                          ? "&comment_author_email=" + HttpUtility.UrlEncode(_AuthorEmail)
                          : "")
                       +
                       (_AuthorUrl != null && _AuthorUrl.Length > 0
                          ? "&comment_author_url=" + HttpUtility.UrlEncode(_AuthorUrl)
                          : "")
                       +
                       (_Content != null && _Content.Length > 0
                          ? "&comment_content=" + HttpUtility.UrlEncode(_Content)
                          : "");


      return encoded;
    }
  }
}