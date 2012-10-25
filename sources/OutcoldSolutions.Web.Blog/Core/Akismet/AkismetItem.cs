// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.Web.Blog.Core.Akismet
{
    using System;
    using System.Net;
    using System.Web;

    /// <summary>
    ///   Represents an Akismet item like a comment or registration data
    /// </summary>
    public class AkismetItem
    {
        private string authorEmail;

        private string authorName;

        private string authorUrl;

        private string content;

        private string ipAddress;

        private string permaLink;

        private string referer;

        private AkismetType type = AkismetType.General;

        private string userAgent;

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
            if (ipAddress == null)
            {
                throw new ArgumentNullException("ipAddress");
            }

            if (userAgent == null)
            {
                throw new ArgumentNullException("userAgent");
            }

            if (ipAddress.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("ipAddress", ipAddress, "ipAddress may not be empty");
            }

            if (userAgent.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("userAgent", userAgent, "userAgent may not be empty");
            }

            this.ipAddress = ipAddress;
            this.userAgent = userAgent;
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
            if (ipAddress == null)
            {
                throw new ArgumentNullException("ipAddress");
            }

            if (userAgent == null)
            {
                throw new ArgumentNullException("userAgent");
            }

            if (userAgent.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("userAgent", userAgent, "userAgent may not be empty");
            }

            this.ipAddress = ipAddress.ToString();
            this.userAgent = userAgent;
        }

        /// <summary>
        ///   IP address of the comment submitter.
        /// </summary>
        /// <exception cref = "ArgumentNullException">IpAddress is null</exception>
        /// <exception cref = "ArgumentOutOfRangeException">IpAddress may not be empty</exception>
        public string IpAddress
        {
            get
            {
                return this.ipAddress;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("IpAddress");
                }

                if (value.Length <= 0)
                {
                    throw new ArgumentOutOfRangeException("IpAddress", value, "IpAddress may not be empty");
                }

                this.ipAddress = value;
            }
        }

        /// <summary>
        ///   User agent information. This value is required!
        /// </summary>
        /// <exception cref = "ArgumentNullException">UserAgent is null</exception>
        /// <exception cref = "ArgumentOutOfRangeException">UserAgent may not be empty</exception>
        public string UserAgent
        {
            get
            {
                return this.userAgent;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("UserAgent");
                }

                if (value.Length <= 0)
                {
                    throw new ArgumentOutOfRangeException("UserAgent", value, "UserAgent may not be empty");
                }

                this.userAgent = value;
            }
        }

        /// <summary>
        ///   The content of the HTTP_REFERER header should be sent here.
        /// </summary>
        public string Referer
        {
            get
            {
                return this.referer;
            }

            set
            {
                this.referer = value;
            }
        }

        /// <summary>
        ///   The permanent location of the entry the comment was submitted to.
        /// </summary>
        public string PermaLink
        {
            get
            {
                return this.permaLink;
            }

            set
            {
                this.permaLink = value;
            }
        }

        /// <summary>
        ///   The type of this comment
        /// </summary>
        public AkismetType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

        /// <summary>
        ///   Submitted name with the comment
        /// </summary>
        public string AuthorName
        {
            get
            {
                return this.authorName;
            }

            set
            {
                this.authorName = value;
            }
        }

        /// <summary>
        ///   Submitted email address
        /// </summary>
        public string AuthorEmail
        {
            get
            {
                return this.authorEmail;
            }

            set
            {
                this.authorEmail = value;
            }
        }

        /// <summary>
        ///   Commenter URL
        /// </summary>
        public string AuthorUrl
        {
            get
            {
                return this.authorUrl;
            }

            set
            {
                this.authorUrl = value;
            }
        }

        /// <summary>
        ///   The comment content
        /// </summary>
        public string Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.content = value;
            }
        }

        /// <summary>
        ///   Returns an UrlEncoded string of all parameters
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string encoded = "&user_agent=" + HttpUtility.UrlEncode(this.userAgent) + "&user_ip="
                             + HttpUtility.UrlEncode(this.ipAddress)
                             + (this.referer != null && this.referer.Length > 0
                                    ? "&referrer=" + HttpUtility.UrlEncode(this.referer)
                                    : string.Empty)
                             + (this.permaLink != null && this.permaLink.Length > 0
                                    ? "&permalink=" + HttpUtility.UrlEncode(this.permaLink)
                                    : string.Empty)
                             + (this.type != AkismetType.General
                                    ? "&comment_type=" + HttpUtility.UrlEncode(this.type.ToString())
                                    : string.Empty)
                             + (this.authorName != null && this.authorName.Length > 0
                                    ? "&comment_author=" + HttpUtility.UrlEncode(this.authorName)
                                    : string.Empty)
                             + (this.authorEmail != null && this.authorEmail.Length > 0
                                    ? "&comment_author_email=" + HttpUtility.UrlEncode(this.authorEmail)
                                    : string.Empty)
                             + (this.authorUrl != null && this.authorUrl.Length > 0
                                    ? "&comment_author_url=" + HttpUtility.UrlEncode(this.authorUrl)
                                    : string.Empty)
                             + (this.content != null && this.content.Length > 0
                                    ? "&comment_content=" + HttpUtility.UrlEncode(this.content)
                                    : string.Empty);

            return encoded;
        }
    }
}