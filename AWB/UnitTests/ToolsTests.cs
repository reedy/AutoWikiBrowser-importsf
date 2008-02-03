﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WikiFunctions;
using System.Text.RegularExpressions;

namespace UnitTests
{
    [TestFixture]
    public class ToolsTests
    {
        [Test]
        public void TestInvalidChars()
        {
            Assert.IsTrue(Tools.IsValidTitle("test"));
            Assert.IsTrue(Tools.IsValidTitle("This is a_test"));
            Assert.IsTrue(Tools.IsValidTitle("123"));
            Assert.IsTrue(Tools.IsValidTitle("А & Б сидели на трубе! ة日?"));

            Assert.IsFalse(Tools.IsValidTitle(""), "Empty strings are not supposed to be valid titles");
            Assert.IsFalse(Tools.IsValidTitle("["));
            Assert.IsFalse(Tools.IsValidTitle("]"));
            Assert.IsFalse(Tools.IsValidTitle("{"));
            Assert.IsFalse(Tools.IsValidTitle("}"));
            Assert.IsFalse(Tools.IsValidTitle("|"));
            Assert.IsFalse(Tools.IsValidTitle("<"));
            Assert.IsFalse(Tools.IsValidTitle(">"));
            Assert.IsFalse(Tools.IsValidTitle("#"));

            //Complex titles
            Assert.IsFalse(Tools.IsValidTitle("[test]#1"));
            Assert.IsFalse(Tools.IsValidTitle("_ _"), "Titles should be normalised before checking");
            Assert.IsTrue(Tools.IsValidTitle("http://www.wikipedia.org")); //unfortunately
            Assert.IsTrue(Tools.IsValidTitle("index.php/Viagra")); //even more unfortunately
            Assert.IsTrue(Tools.IsValidTitle("index.php?title=foobar"));
        }

        [Test]
        public void RemoveInvalidChars()
        {
            Assert.AreEqual(Tools.RemoveInvalidChars("tesT 123!"), "tesT 123!");
            Assert.AreEqual(Tools.RemoveInvalidChars("тест, ёпта"), "тест, ёпта");
            Assert.AreEqual(Tools.RemoveInvalidChars(""), "");
            Assert.AreEqual(Tools.RemoveInvalidChars("{<[test]>}"), "test");
            Assert.AreEqual(Tools.RemoveInvalidChars("#|#"), "");
            Assert.AreEqual(Tools.RemoveInvalidChars("http://www.wikipedia.org"), "http://www.wikipedia.org");
        }

        [Test]
        public void TestRomanNumbers()
        {
            Assert.IsTrue(Tools.IsRomanNumber("XVII"));
            Assert.IsTrue(Tools.IsRomanNumber("I"));

            Assert.IsFalse(Tools.IsRomanNumber("xvii"));
            Assert.IsFalse(Tools.IsRomanNumber("XXXXXX"));
            Assert.IsFalse(Tools.IsRomanNumber("V II"));
            Assert.IsFalse(Tools.IsRomanNumber("AAA"));
            Assert.IsFalse(Tools.IsRomanNumber("123"));
            Assert.IsFalse(Tools.IsRomanNumber(" "));
            Assert.IsFalse(Tools.IsRomanNumber(""));
        }

        [Test]
        public void TestCaseInsensitive()
        {
            Assert.AreEqual("", Tools.CaseInsensitive(""));
            Assert.AreEqual("123", Tools.CaseInsensitive("123"));
            Assert.AreEqual("-", Tools.CaseInsensitive("-"));

            Regex r = new Regex(Tools.CaseInsensitive("test"));
            Assert.IsTrue(r.IsMatch("test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("tEst"));

            r = new Regex(Tools.CaseInsensitive("Test"));
            Assert.IsTrue(r.IsMatch("test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("TEst"));

            r = new Regex(Tools.CaseInsensitive("[test}"));
            Assert.IsTrue(r.IsMatch("[test}"));
            Assert.IsFalse(r.IsMatch("[Test}"));
            Assert.IsFalse(r.IsMatch("test"));
        }
    }
}
