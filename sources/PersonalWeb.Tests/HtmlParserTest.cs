using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using PersonalWeb.Core;

// ReSharper disable InconsistentNaming
namespace PersonalWeb.Tests
{
	///<summary>
	///	This is a test class for TwitterRepositoryTest and is intended
	///	to contain all TwitterRepositoryTest Unit Tests
	///</summary>
	[TestFixture]
	public class HtmlParserTest
	{
		[Test]
		public void from_comment_to_html_do_br()
		{
			string comment = @"Hello 
How are you?";

			string htmlComment = HtmlParser.DoBr(comment);
			Assert.AreEqual("Hello <br />How are you?", htmlComment);
		}

		[Test]
		public void add_http()
		{
			string comment = @"outcoldman.ru";

			string htmlComment = HtmlParser.AddHttp(comment);
			Assert.AreEqual("http://outcoldman.ru", htmlComment);
		}

		[Test]
		public void FindCode_CodeExists_CodeHiglighted()
		{
			string comment =
				@" sd sd [code]	format.Editor.WordWrap = false;[/code] sds ds
sd sd [code]format.Editor.WordWrap = false;
[/code] sds ds";

			string highlightCode = HtmlParser.HighlightCode(comment);
			
			Assert.IsNotEmpty(highlightCode);
		}

		// ReSharper restore InconsistentNaming
	}
}