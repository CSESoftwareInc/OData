﻿using System;
using CSESoftware.OData.Builder;
using CSESoftware.OData.FrameworkTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSESoftware.OData.FrameworkTests
{
    [TestClass]
    public class FilterBuilderTests
    {
        [TestMethod]
        public void WithMixedObjectTypes()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Comment, Operation.Contains, "m22")
                .AndWhere(x => x.EmployeeId, Operation.Equals, 5)
                .BuildObject();

            Assert.AreEqual("(contains(Comment, 'm22')) and (EmployeeId eq 5)", filter.Filter);
        }

        [TestMethod]
        public void WithIdBuildTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .WhereIdIs(7)
                .BuildObject();

            Assert.AreEqual("Id eq 7", filter.Filter);
        }

        [TestMethod]
        public void WithIdQueryStringTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .WhereIdIs(7)
                .Build();

            Assert.AreEqual("?$filter=Id eq 7", filter);
        }

        [TestMethod]
        public void WhereExpressionTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.EmployeeId, Operation.NotEqualTo, 3)
                .BuildObject();

            Assert.AreEqual("(EmployeeId ne 3)", filter.Filter);
        }

        [TestMethod]
        public void OrWhereExpressionTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Id, Operation.Equals, 8)
                .OrWhere(x => x.EmployeeId, Operation.Equals, 37)
                .BuildObject();

            Assert.AreEqual("(Id eq 8) or (EmployeeId eq 37)", filter.Filter);
        }

        [TestMethod]
        public void AndWhereExpressionTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Id, Operation.Equals, 8)
                .AndWhere(x => x.EmployeeId, Operation.Equals, 37)
                .BuildObject();

            Assert.AreEqual("(Id eq 8) and (EmployeeId eq 37)", filter.Filter);
        }

        [TestMethod]
        public void WhereBetweenTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .WhereBetween(x => x.Id, 3, 7)
                .BuildObject();

            Assert.AreEqual("(Id ge 3 and Id le 7)", filter.Filter);
        }

        [TestMethod]
        public void WhereExclusiveBetweenTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .WhereExclusiveBetween(x => x.Id, 3, 7)
                .BuildObject();

            Assert.AreEqual("(Id gt 3 and Id lt 7)", filter.Filter);
        }

        [TestMethod]
        public void ContainsOperationTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Comment, Operation.Contains, "asdf")
                .BuildObject();

            Assert.AreEqual("(contains(Comment, 'asdf'))", filter.Filter);
        }

        [TestMethod]
        public void StringObjectTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.Comment, Operation.Equals, "xkcd")
                .BuildObject();

            Assert.AreEqual("(Comment eq 'xkcd')", filter.Filter);
        }

        [TestMethod]
        public void DateTimeObjectTest()
        {
            var date = new DateTime(2015, 3, 14, 9, 26, 53);

            var filter = new ODataFilterBuilder<Timesheet>()
                .Where(x => x.CreatedDate, Operation.GreaterThan, date)
                .BuildObject();

            Assert.AreEqual("(CreatedDate gt 2015-03-14T09:26:53.0000000)", filter.Filter);
        }

        [TestMethod]
        public void OrderByTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .OrderBy(x => x.StartTime)
                .BuildObject();

            Assert.AreEqual("StartTime", filter.OrderBy);
        }

        [TestMethod]
        public void OrderByDescendingTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .OrderBy(x => x.StartTime, true)
                .BuildObject();

            Assert.AreEqual("StartTime desc", filter.OrderBy);
        }

        [TestMethod]
        public void OrderByChildPropertyTest()
        {
            var filter = new ODataFilterBuilder<Timesheet>()
                .OrderBy(x => x.Employee.Name)
                .BuildObject();

            Assert.AreEqual("Employee.Name", filter.OrderBy);
        }
    }
}
