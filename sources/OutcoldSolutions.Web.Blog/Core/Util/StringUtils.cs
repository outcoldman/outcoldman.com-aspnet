// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Core.Util
{
    public static class StringUtils
    {
        public static string SafelySubstring(this string str, int lenght)
        {
            if (str.Length > lenght)
            {
                return str.Substring(0, lenght);
            }

            return str;
        }
    }
}