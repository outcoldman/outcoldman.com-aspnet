using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Tests
{
	[TestFixture]
	class ParserSpec
	{
		// ReSharper disable InconsistentNaming

		[Test]
		public void parse_string_to_int()
		{
			int i = "1".To<int>();
			Assert.AreEqual(i, 1);
		}

		[Test]
		public void cant_parse_string_to_int()
		{
			int i = "1a".To<int>();
			Assert.AreEqual(i, 0);
		}

		[Test]
		public void cant_parse_string_to_int_with_default()
		{
			int i = "1a".To(10);
			Assert.AreEqual(i, 10);
		}

		[Test]
		public void parse_string_to_int_with_defaultT()
		{
			int i = "1".To(10);
			Assert.AreEqual(i, 1);
		}

		[Test]
		public void parse_string_to_nullable_int()
		{
			int? i = "1".To<int?>();
			Assert.AreEqual(i, 1);
		}

		[Test]
		public void cant_parse_string_to_nullable_int()
		{
			int? i = "1a".To<int?>();
			Assert.AreEqual(i, null);
		}

		[Test]
		public void cant_parse_string_to_nullable_int_with_default()
		{
			int? i = "1a".To<int?>(10);
			Assert.AreEqual(i, 10);
		}

		[Test]
		public void cant_parse_string_to_nullable_int_with_nullable_default()
		{
			int? i = "1a".To<int?>(10);
			Assert.AreEqual(i, 10);
		}

		[Test]
		public void parse_string_to_nullable_int_with_defaultT()
		{
			int? i = "1".To<int?>(10);
			Assert.AreEqual(i, 1);
		}

		[Test]
		public void parse_to_enum()
		{
			UriFormat i = "UriEscaped".To<UriFormat>();
			Assert.AreEqual(i, UriFormat.UriEscaped);
		}

		[Test]
		public void cant_parse_to_enum()
		{
			UriFormat i = "UriEscaped1".To<UriFormat>();
			Assert.AreEqual(i, default(UriFormat));
		}

		[Test]
		public void cant_parse_to_enum_with_defalt()
		{
			UriFormat i = "UriEscaped1".To(UriFormat.Unescaped);
			Assert.AreEqual(i, UriFormat.Unescaped);
		}

		[Test]
		public void parse_to_bool()
		{
			bool i = "true".To<bool>();
			Assert.AreEqual(i, true);
		}

		[Test]
		public void parse_object_to_string()
		{
			var a = new {a = "aa", b = "bb"};
			string i = a.To<string>();
			Assert.AreEqual("{ a = aa, b = bb }", i);
		}

		// ReSharper enable InconsistentNaming
	}
}
