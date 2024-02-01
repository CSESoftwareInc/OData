using CSESoftware.OData.Tests.Helpers;

namespace CSESoftware.OData.Tests
{
    public class StringConverterTests : ODataRepository
    {
        public StringConverterTests() : base(null) { }

        [Fact]
        public void EmptyContainsTest()
        {
            var filter = "contains(Text, '')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text, \"\")", filter);

            filter = "contains(Text,'')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text,\"\")", filter);
        }

        [Fact]
        public void RegularContainsTest()
        {
            var filter = "contains(Text, 'search')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text, \"search\")", filter);

            filter = "contains(Text,'search')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text,\"search\")", filter);
        }

        [Fact]
        public void ContainsEndingInApostropheTest()
        {
            var filter = "contains(Text, 'don'')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text, \"don'\")", filter);

            filter = "contains(Text,'don'')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text,\"don'\")", filter);
        }

        [Fact]
        public void ContainsWithApostropheTest()
        {
            var filter = "contains(Text, 'don't')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text, \"don't\")", filter);

            filter = "contains(Text,'don't')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text,\"don't\")", filter);
        }

        [Fact]
        public void ContainsWithMultipleApostrophesTest()
        {
            var filter = "contains(Text, 'don't we'll')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text, \"don't we'll\")", filter);

            filter = "contains(Text,'don't we'll')";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("contains(Text,\"don't we'll\")", filter);
        }

        [Fact]
        public void EmptyWhere()
        {
            var filter = "Text eq ''";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("Text eq \"\"", filter);
        }

        [Fact]
        public void RegularWhere()
        {
            var filter = "Text eq 'search'";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("Text eq \"search\"", filter);
        }

        [Fact]
        public void WhereEndingInApostropheTest()
        {
            var filter = "Text eq 'don''";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("Text eq \"don'\"", filter);
        }

        [Fact]
        public void WhereWithApostrophe()
        {
            var filter = "Text eq 'don't'";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("Text eq \"don't\"", filter);
        }

        [Fact]
        public void WhereWithMultipleApostrophes()
        {
            var filter = "Text eq 'don't we'll'";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("Text eq \"don't we'll\"", filter);
        }

        [Fact]
        public void WhereWithQuotes()
        {
            var filter = "Text eq 'he said \"yes\"'";
            filter = ConvertStringsToAppropriateFormat(filter);
            Assert.Equal("Text eq \"he said \\\"yes\\\"\"", filter);
        }

        [Fact]
        public void WhereWithStartAndEndQuotes()
        {
            var filter = "Name eq '\"Bob\"'";
            var expression = GenerateExpressionFilter<Employee>(filter);
            Assert.Equal("(entity.Name == \"\"Bob\"\")", expression.Body.ToString());
        }

        [Fact]
        public void FullExpressionTest()
        {
            var filter = "Name eq 'Bob'";
            var expression = GenerateExpressionFilter<Employee>(filter);
            Assert.Equal("(entity.Name == \"Bob\")", expression.Body.ToString());
        }

        [Fact]
        public void TwoStringExpressionsTest()
        {
            var filter = "Name eq 'Bob' and Address eq 'Boo'";
            var expression = GenerateExpressionFilter<Employee>(filter);
            Assert.Equal("((entity.Name == \"Bob\") AndAlso (entity.Address == \"Boo\"))", expression.Body.ToString());
        }
    }
}
