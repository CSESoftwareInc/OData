﻿using CSESoftware.OData.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSESoftware.OData.Tests
{
    [TestClass]
    public class StringConverterTests : ODataRepository
    {
        public StringConverterTests() : base(null) { }

        [TestMethod]
        public void EmptyContainsTest()
        {
            var filter = "contains(Text, '')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text, \"\")", filter);

            filter = "contains(Text,'')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text,\"\")", filter);
        }

        [TestMethod]
        public void RegularContainsTest()
        {
            var filter = "contains(Text, 'search')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text, \"search\")", filter);

            filter = "contains(Text,'search')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text,\"search\")", filter);
        }

        [TestMethod]
        public void ContainsEndingInApostropheTest()
        {
            var filter = "contains(Text, 'don'')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text, \"don'\")", filter);

            filter = "contains(Text,'don'')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text,\"don'\")", filter);
        }

        [TestMethod]
        public void ContainsWithApostropheTest()
        {
            var filter = "contains(Text, 'don't')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text, \"don't\")", filter);

            filter = "contains(Text,'don't')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text,\"don't\")", filter);
        }

        [TestMethod]
        public void ContainsWithMultipleApostrophesTest()
        {
            var filter = "contains(Text, 'don't we'll')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text, \"don't we'll\")", filter);

            filter = "contains(Text,'don't we'll')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("contains(Text,\"don't we'll\")", filter);
        }

        [TestMethod]
        public void EmptyWhere()
        {
            var filter = "Text eq ''";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("Text eq \"\"", filter);
        }

        [TestMethod]
        public void RegularWhere()
        {
            var filter = "Text eq 'search'";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("Text eq \"search\"", filter);
        }

        [TestMethod]
        public void WhereEndingInApostropheTest()
        {
            var filter = "Text eq 'don''";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("Text eq \"don'\"", filter);
        }

        [TestMethod]
        public void WhereWithApostrophe()
        {
            var filter = "Text eq 'don't'";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("Text eq \"don't\"", filter);
        }

        [TestMethod]
        public void WhereWithMultipleApostrophes()
        {
            var filter = "Text eq 'don't we'll'";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("Text eq \"don't we'll\"", filter);
        }

        [TestMethod]
        public void WhereWithQuotes()
        {
            var filter = "Text eq 'he said \"yes\"'";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.AreEqual("Text eq \"he said \\\"yes\\\"\"", filter);
        }

        [TestMethod]
        public void WhereWithStartAndEndQuotes()
        {
            var filter = "Name eq '\"Bob\"'";
            var expression = GenerateExpressionFilter<Employee>(filter);
            Assert.AreEqual("(entity.Name == \"\"Bob\"\")", expression.Body.ToString());
        }

        [TestMethod]
        public void FullExpressionTest()
        {
            var filter = "Name eq 'Bob'";
            var expression = GenerateExpressionFilter<Employee>(filter);
            Assert.AreEqual("(entity.Name == \"Bob\")", expression.Body.ToString());
        }

        [TestMethod]
        public void TwoStringExpressionsTest()
        {
            var filter = "Name eq 'Bob' and Address eq 'Boo'";
            var expression = GenerateExpressionFilter<Employee>(filter);
            Assert.AreEqual("((entity.Name == \"Bob\") AndAlso (entity.Address == \"Boo\"))", expression.Body.ToString());
        }
    }
}
