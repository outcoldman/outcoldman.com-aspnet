// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.Web.Blog.Core.Akismet
{
    using System;

    /// <summary>
    /// General exception which occured in Akismet
    /// </summary>
    public class AkismetException : ApplicationException
    {
        internal AkismetException(string msg)
            : base(msg)
        {
        }

        internal AkismetException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }
    }
}