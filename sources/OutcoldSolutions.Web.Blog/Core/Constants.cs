namespace OutcoldSolutions.Web.Blog.Core
{
    public static class Constants
    {
		public const string RegexUrl = @"(https?://(www.)?([\w\-]+)(\.([\w\-]+))+([\w\\\/\.?&%=\-+]*))";
        public const string RegexTwitterUser = @"(@([A-Za-z0-9_]+))";
        public const string RegexTwitterTag = @"(#([A-Za-z0-9_]+))";
        public const string RegexIsRussian = @"[А-Яа-я]+";
    }
}