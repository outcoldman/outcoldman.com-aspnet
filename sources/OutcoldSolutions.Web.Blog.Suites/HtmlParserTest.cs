// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Suites
{
    using NUnit.Framework;

    using OutcoldSolutions.Web.Blog.Core;

    /// <summary>
    /// This is a test class for TwitterRepositoryTest and is intended
    /// to contain all TwitterRepositoryTest Unit Tests
    /// </summary>
    [TestFixture]
    public class HtmlParserTest
    {
        [Test]
        public void FindCode_CodeExists_CodeHiglighted()
        {
            string comment = @" sd sd [code]	format.Editor.WordWrap = false;[/code] sds ds
sd sd [code]format.Editor.WordWrap = false;
[/code] sds ds";

            string highlightCode = HtmlParser.HighlightCode(comment);

            Assert.IsNotEmpty(highlightCode);
        }

        [Test]
        public void Add_Http()
        {
            string comment = @"outcoldman.ru";

            string htmlComment = HtmlParser.AddHttp(comment);
            Assert.AreEqual("http://outcoldman.ru", htmlComment);
        }

        [Test]
        public void From_Comment_To_Html_Do_Br()
        {
            string comment = @"Hello 
How are you?";

            string htmlComment = HtmlParser.DoBr(comment);
            Assert.AreEqual("Hello <br /><br />How are you?", htmlComment);
        }
    }
}