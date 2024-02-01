using CSESoftware.OData.Builder;
using CSESoftware.OData.Tests.Helpers;

namespace CSESoftware.OData.Tests
{
    public class FilterBuilderTests
    {
        [Fact]
        public void WithMixedObjectTypes()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Comment, Operation.Contains, "m22")
                .AndWhere(x => x.EmployeeId, Operation.Equals, 5)
                .BuildObject();

            Assert.Equal("(contains(Comment, 'm22')) and (EmployeeId eq 5)", filter.Filter);
        }

        [Fact]
        public void WithIdBuildTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .WhereIdIs(7)
                .BuildObject();

            Assert.Equal("Id eq 7", filter.Filter);
        }

        [Fact]
        public void WithIdQueryStringTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .WhereIdIs(7)
                .Build();

            Assert.Equal("?$filter=Id eq 7", filter);
        }

        [Fact]
        public void WhereExpressionTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.EmployeeId, Operation.NotEqualTo, 3)
                .BuildObject();

            Assert.Equal("(EmployeeId ne 3)", filter.Filter);
        }

        [Fact]
        public void OrWhereExpressionTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Id, Operation.Equals, 8)
                .OrWhere(x => x.EmployeeId, Operation.Equals, 37)
                .BuildObject();

            Assert.Equal("(Id eq 8) or (EmployeeId eq 37)", filter.Filter);
        }

        [Fact]
        public void AndWhereExpressionTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Id, Operation.Equals, 8)
                .AndWhere(x => x.EmployeeId, Operation.Equals, 37)
                .BuildObject();

            Assert.Equal("(Id eq 8) and (EmployeeId eq 37)", filter.Filter);
        }

        [Fact]
        public void WhereBetweenTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .WhereBetween(x => x.Id, 3, 7)
                .BuildObject();

            Assert.Equal("(Id ge 3 and Id le 7)", filter.Filter);
        }

        [Fact]
        public void WhereExclusiveBetweenTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .WhereExclusiveBetween(x => x.Id, 3, 7)
                .BuildObject();

            Assert.Equal("(Id gt 3 and Id lt 7)", filter.Filter);
        }

        [Fact]
        public void ContainsOperationTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Comment, Operation.Contains, "asdf")
                .BuildObject();

            Assert.Equal("(contains(Comment, 'asdf'))", filter.Filter);
        }

        [Fact]
        public void StringObjectTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Comment, Operation.Equals, "xkcd")
                .BuildObject();

            Assert.Equal("(Comment eq 'xkcd')", filter.Filter);
        }

        [Fact]
        public void DateTimeObjectTest()
        {
            var date = new DateTime(2015, 3, 14, 9, 26, 53);

            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.CreatedDate, Operation.GreaterThan, date)
                .BuildObject();

            Assert.Equal("(CreatedDate gt 2015-03-14T09:26:53.0000000)", filter.Filter);
        }

        [Fact]
        public void OrderByTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .OrderBy(x => x.StartTime)
                .BuildObject();

            Assert.Equal("StartTime", filter.OrderBy);
        }

        [Fact]
        public void OrderByDescendingTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .OrderBy(x => x.StartTime, true)
                .BuildObject();

            Assert.Equal("StartTime desc", filter.OrderBy);
        }

        [Fact]
        public void OrderByChildPropertyTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .OrderBy(x => x.Employee.Name)
                .BuildObject();

            Assert.Equal("Employee.Name", filter.OrderBy);
        }
    }
}
