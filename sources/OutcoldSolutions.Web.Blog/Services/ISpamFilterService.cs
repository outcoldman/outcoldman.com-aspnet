// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Services
{
    public interface ISpamFilterService
    {
        bool IsSpam(
            string comment,
            string userIP,
            string userAgent,
            string userName = null,
            string userEmail = null,
            string userWeb = null);

        void Spam(
            string comment,
            string userIP,
            string userAgent,
            string userName = null,
            string userEmail = null,
            string userWeb = null);

        void NotSpam(
            string comment,
            string userIP,
            string userAgent,
            string userName = null,
            string userEmail = null,
            string userWeb = null);
    }
}