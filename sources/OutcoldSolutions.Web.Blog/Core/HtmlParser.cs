namespace OutcoldSolutions.Web.Blog.Core
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class HtmlParser
	{
		public static string DoBr(string text)
		{
			//text = Regex.Replace(text, "<!--.*-->", string.Empty);
			//text = Regex.Replace(text, string.Format("({0}|{1})[^<]", Environment.NewLine, "\n"), "<br />");
			text = Regex.Replace(text, string.Format("(</?[ib][^>]?>|[^>\r\n])({0}|{1})+(<[ib]|[^<\r\n])", Environment.NewLine, "\n"), "$1<br /><br />$3");
			return text;
			//return Regex.Replace(text, "(</?[^/bis][^>]*>)([ \t]*<br />[ \t]*)+([ \t]*<[^bis])", "$1$3", RegexOptions.IgnoreCase);
		}

		public static string AddHttp(string site)
		{
			if (!string.IsNullOrEmpty(site) && !site.StartsWith("http"))
			{
				site = "http://" + site;
			}
			return site;
		}

		public static string HighlightCode(string input)
		{
			if (!input.Contains("[code"))
				return input;

			SourceCodeHighlighterService service = new SourceCodeHighlighterService();

			StringBuilder result = new StringBuilder();

			for(int i = 0; i < input.Length; i++)
			{
				if (input[i] == '[')
				{
					i += 5;
					if (string.Compare(input.Substring(i-5, 5), "[code", true) == 0)
					{
						StringBuilder type = new StringBuilder();

						while (input[i] != ']')
						{
							type.Append(input[i++]);
							if (i >= input.Length)
							{
								result.Append(input.Substring(i - 5, 5));
								result.Append(type);
								type = null;
								break;
							}
						}
						if (type != null)
						{
							i++;
							StringBuilder code = new StringBuilder();
							while (input[i] != '['
								|| Char.ToLower(input[i + 1]) != '/'
								|| Char.ToLower(input[i + 2]) != 'c'
								|| Char.ToLower(input[i + 3]) != 'o'
								|| Char.ToLower(input[i + 4]) != 'd'
								|| Char.ToLower(input[i + 5]) != 'e'
								|| Char.ToLower(input[i + 6]) != ']'
								)
							{
								code.Append(input[i++]);
								if (i >= input.Length)
								{
									result.Append(code);
									code = null;
									break;
								}
							}

							if (code != null)
							{
								i += 6;
								result.Append(service.HighlightCode(code.ToString().Trim(' ', '\r', '\n'), type.ToString().Trim()));
							}
						}
						
					}
					
					continue;
				}
				
				result.Append(input[i]);
			}

			return result.ToString();
		}
	}
}
