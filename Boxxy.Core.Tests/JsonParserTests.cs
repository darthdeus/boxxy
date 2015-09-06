using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boxxy.Core.Tests
{
    [TestClass]
    public class JsonParserTests
    {
        [TestMethod]
        public void EmptyString() {
            var res1 = Json.String().Run("\"\"");
            Assert.IsTrue(res1.Success);
            Assert.AreEqual("", res1.Result.Item1.Value);
        }

        [TestMethod]
        public void StringTest() {
            var res1 = Json.String().Run("\"hello\"");

            Console.WriteLine(res1.Error);
            Assert.IsTrue(res1.Success);
            Assert.AreEqual("hello", res1.Result.Item1.Value);

            var res2 = Json.String().Run("\"hell\\\"o\"");

            Assert.IsTrue(res2.Success);
            Assert.AreEqual("hell\\\"o", res2.Result.Item1.Value);
        }

        [TestMethod]
        public void NumberTest() {
            var res1 = Json.Number().Run("123");

            Assert.IsTrue(res1.Success);
            Assert.AreEqual(123d, res1.Result.Item1.Value);

            var res2 = Json.Number().Run("123.456");

            Assert.IsTrue(res2.Success);
            Assert.AreEqual(123.456, res2.Result.Item1.Value);
        }

        [TestMethod]
        public void ObjectTest() {
            var res1 = Json.Object().Run("{\"foo\": 123, \"bar\": \"baz\"}");
            
            Assert.IsTrue(res1.Success);
            Assert.AreEqual(123, ((JsonNumber)res1.Result.Item1.Value["foo"]).Value);
            Assert.AreEqual("baz", ((JsonString)res1.Result.Item1.Value["bar"]).Value);
        }
    }
}
