// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Suites
{
    using NUnit.Framework;

    using OutcoldSolutions.Web.Blog.Core;

    [TestFixture]
    internal class DoBrSpec
    {
        [Test]
        public void DoBr()
        {
            string test = @"text	
textt";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual("text\t<br /><br />textt", doBr);
        }

        [Test]
        public void DoBr_10()
        {
            string test = @"</pre>
<!--CRLF-->

	<pre";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual(@"</pre>
<!--CRLF-->

	<pre", doBr);
        }

        [Test]
        public void DoBr_3()
        {
            string test = @"text	


textt";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual("text\t<br /><br />textt", doBr);
        }

        [Test]
        public void DoBr_4()
        {
            string test = @"</b>


<i>";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual("</b><br /><br /><i>", doBr);
        }

        [Test]
        public void DoBr_5()
        {
            string test = @"</b>


text";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual("</b><br /><br />text", doBr);
        }

        [Test]
        public void DoBr_6()
        {
            string test = @"text


<i>";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual("text<br /><br /><i>", doBr);
        }

        [Test]
        public void DoBr_7()
        {
            string test = @"</p>


<i>";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual(@"</p>


<i>", doBr);
        }

        [Test]
        public void DoBr_8()
        {
            string test = @"</i>


<p>";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual(@"</i>


<p>", doBr);
        }

        [Test]
        public void DoBr_9()
        {
            string test = @"</p><!--CRLF-->


<p>";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual(@"</p><!--CRLF-->


<p>", doBr);
        }

        [Test]
        public void DoBr_Double()
        {
            string test = @"text	

textt";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual("text\t<br /><br />textt", doBr);
        }

        [Test]
        public void DoBr_p()
        {
            string test = @"<p>text</p>
<p>textt</p>";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual(@"<p>text</p>
<p>textt</p>", doBr);
        }

        [Test]
        public void DoBr_p_double()
        {
            string test = @"<p>text</p>

<p>textt</p>";
            string doBr = HtmlParser.DoBr(test);
            Assert.AreEqual(@"<p>text</p>

<p>textt</p>", doBr);
        }
    }
}