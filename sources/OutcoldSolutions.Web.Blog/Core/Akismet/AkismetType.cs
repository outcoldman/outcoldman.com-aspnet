// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core.Akismet
{
    /// <summary>
    /// Type of the item you want to check
    /// </summary>
    public enum AkismetType
    {
        /// <summary>
        /// Default
        /// </summary>
        General = 0, 

        /// <summary>
        /// Comments
        /// </summary>
        Comment = 1, 

        /// <summary>
        /// Trackbacks
        /// </summary>
        Trackback = 2, 

        /// <summary>
        /// Pingbacks
        /// </summary>
        Pingback = 3, 

        /// <summary>
        /// Registration forms
        /// </summary>
        Registration = 4
    }
}