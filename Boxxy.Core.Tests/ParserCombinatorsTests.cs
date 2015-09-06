using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boxxy.Core.Tests
{
    [TestClass]
    public class ParserCombinatorsTests
    {
        [TestMethod]
        public void ResultHelpersTest() {
            var why = "unexpected char";
            var result = ParseResult.Failed<int>(why);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(why, result.Error);

            var okValue = 1;
            var result2 = ParseResult.Passed(okValue, "");
            Assert.AreEqual(okValue, result2.Result.Item1);
        }

        [TestMethod]
        public void CharParserTest() {
            var res1 = Parser.Char('c').Run("hello");
            Assert.IsFalse(res1.Success);

            var res2 = Parser.Char('c').Run("cello");
            Assert.IsTrue(res2.Success);
            Assert.AreEqual('c', res2.Result.Item1);
            Assert.AreEqual("ello", res2.Result.Item2);
        }

        [TestMethod]
        public void CombineTest() {
            var res1 = Parser.Combine(Parser.Char('c'), Parser.Char('e')).Run("cello");

            Assert.IsTrue(res1.Success);
            Assert.AreEqual('e', res1.Result.Item1);
            Assert.AreEqual("llo", res1.Result.Item2);
        }

        [TestMethod]
        public void StringTest() {
            var res = Parser.String("hell").Run("helloo");

            Assert.IsTrue(res.Success);
            Assert.AreEqual("hell", res.Result.Item1);
            Assert.AreEqual("oo", res.Result.Item2);
        }

        [TestMethod]
        public void EscapedStringTest() {
            var res1 = Parser.EscapedString('"').Run("\"\"");
            Assert.IsTrue(res1.Success);
            Assert.AreEqual("", res1.Result.Item1);

            var res2 = Parser.EscapedString('"').Run("\"foo\"");
            Assert.IsTrue(res2.Success);
            Assert.AreEqual("foo", res2.Result.Item1);

            var res3 = Parser.EscapedString('"').Run("\"f\\\"");
            Assert.IsTrue(res3.Success);
            Assert.AreEqual("f\\\"", res3.Result.Item1);

            var res4 = Parser.EscapedString('"').Run("\"f\\oo\"");
            Assert.IsTrue(res4.Success);
            Assert.AreEqual("f\\oo", res4.Result.Item1);

            var res5 = Parser.EscapedString('"').Run("\"f\\\"oo\"");
            Assert.IsTrue(res5.Success);
            Assert.AreEqual("f\\\"oo", res5.Result.Item1);
        }

        [TestMethod]
        public void OrTest() {
            var res1 = Parser.Or(Parser.Char('a'), Parser.Char('b')).Run("a");

            Assert.IsTrue(res1.Success);
            Assert.AreEqual('a', res1.Result.Item1);

            var res2 = Parser.Or(Parser.Char('a'), Parser.Char('b')).Run("b");

            Assert.IsTrue(res2.Success);
            Assert.AreEqual('b', res2.Result.Item1);
        }

        [TestMethod]
        public void NumberTest() {
            var res = Parser.Number().Run("123");

            Assert.IsTrue(res.Success);
            Assert.AreEqual(123, res.Result.Item1);
            Assert.AreEqual("", res.Result.Item2);
        }

        [TestMethod]
        public void BracketTest() {
            var res = Parser.Bracket(
                Parser.Char('('),
                Parser.Number(),
                Parser.Char(')'))
                .Run("(123)");
        }

        [TestMethod]
        public void OneOfTest() {
            var res = Parser.OneOf("abc").Run("c");

            Assert.IsTrue(res.Success);
            Assert.AreEqual('c', res.Result.Item1);
        }

        [TestMethod]
        public void DoubleTest() {
            var res = Parser.Double().Run("123.456");

            Assert.IsTrue(res.Success);
            Assert.AreEqual(res.Result.Item1, 123.456);
        }
    }
}