using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CSESoftware.OData.Filter;

namespace CSESoftware.OData.Builder
{
    public class ODataFilterBuilder<T>
    {
        private readonly IODataFilter _openDataFilter;

        public ODataFilterBuilder()
        {
            _openDataFilter = new ODataFilter();
        }

        /// <summary>
        /// Set the filter to a string expression in the form of "Parameter Operator Value"
        /// ex. "Id eq 7"
        /// </summary>
        public ODataFilterBuilder<T> Where(string expression)
        {
            _openDataFilter.Filter = $"({expression})";
            return this;
        }

        /// <summary>
        /// Set the filter based on a property, operation, value
        /// ex. (x => x.Id, Operation.Equals, 7)
        /// </summary>
        public ODataFilterBuilder<T> Where(Expression<Func<T, object>> property, Operation operation, object value)
        {
            return Where(ExpressionToString(property, operation, value));
        }

        /// <summary>
        /// Expand the current filter with an optional where expression
        /// </summary>
        public ODataFilterBuilder<T> OrWhere(string expression)
        {
            if (string.IsNullOrWhiteSpace(_openDataFilter.Filter)) return Where(expression);

            _openDataFilter.Filter += $" or ({expression})";
            return this;
        }

        /// <summary>
        /// Expand the current filter with an optional where expression
        /// </summary>
        public ODataFilterBuilder<T> OrWhere(Expression<Func<T, object>> property, Operation operation, object value)
        {
            return OrWhere(ExpressionToString(property, operation, value));
        }

        /// <summary>
        /// Expand the current filter with a required where expression
        /// </summary>
        public ODataFilterBuilder<T> AndWhere(string expression)
        {
            if (string.IsNullOrWhiteSpace(_openDataFilter.Filter)) return Where(expression);

            _openDataFilter.Filter += $" and ({expression})";
            return this;
        }

        /// <summary>
        /// Expand the current filter with a required where expression
        /// </summary>
        public ODataFilterBuilder<T> AndWhere(Expression<Func<T, object>> property, Operation operation, object value)
        {
            return AndWhere(ExpressionToString(property, operation, value));
        }

        /// <summary>
        /// Set the filter to search for a range of values including the upper and lower bound
        /// </summary>
        public ODataFilterBuilder<T> WhereBetween(Expression<Func<T, object>> property, object lowerBound, object upperBound)
        {
            _openDataFilter.Filter = $"({ExpressionToString(property, Operation.GreaterThanOrEqualTo, lowerBound)} " +
                $"and {ExpressionToString(property, Operation.LessThanOrEqualTo, upperBound)})";
            return this;
        }

        /// <summary>
        /// Set the filter to search for a range of values excluding the upper and lower bound
        /// </summary>
        public ODataFilterBuilder<T> WhereExclusiveBetween(Expression<Func<T, object>> property, object lowerBound, object upperBound)
        {
            _openDataFilter.Filter = $"({ExpressionToString(property, Operation.GreaterThan, lowerBound)} " +
                $"and {ExpressionToString(property, Operation.LessThan, upperBound)})";
            return this;
        }

        /// <summary>
        /// Set the filter to search for a specific ID
        /// </summary>
        public ODataFilterBuilder<T> WhereIdIs(object id)
        {
            _openDataFilter.Filter = $"Id eq {id}";
            return this;
        }

        /// <summary>
        /// Set the take value on the filter
        /// </summary>
        public ODataFilterBuilder<T> Take(int take)
        {
            _openDataFilter.Take = take;
            return this;
        }

        /// <summary>
        /// Set the skip value of the filter
        /// </summary>
        public ODataFilterBuilder<T> Skip(int skip)
        {
            _openDataFilter.Skip = skip;
            return this;
        }

        /// <summary>
        /// Set the property to order the results by
        /// </summary>
        /// <param name="descending">True if results should be in descending order</param>
        public ODataFilterBuilder<T> OrderBy(string property, bool descending = false)
        {
            if (descending)
                property += " desc";

            _openDataFilter.OrderBy = property;
            return this;
        }

        /// <summary>
        /// Set the property to order the results by
        /// </summary>
        /// <param name="descending">True if results should be in descending order</param>
        public ODataFilterBuilder<T> OrderBy(Expression<Func<T, object>> property, bool descending = false)
        {
            return OrderBy(GetMemberName(property), descending);
        }

        /// <summary>
        /// Set the second property to order the results by
        /// </summary>
        /// <param name="descending">True if results should be in descending order</param>
        public ODataFilterBuilder<T> ThenBy(string property, bool descending = false)
        {
            if (descending)
                property += " desc";

            if (_openDataFilter.ThenBy != null)
                property = $",{property}";

            _openDataFilter.ThenBy += property;
            return this;
        }

        /// <summary>
        /// Set the second property to order the results by
        /// </summary>
        /// <param name="descending">True if results should be in descending order</param>
        public ODataFilterBuilder<T> ThenBy(Expression<Func<T, object>> property, bool descending = false)
        {
            return ThenBy(GetMemberName(property), descending);
        }

        /// <summary>
        /// Add property to list of objects to expand in the result
        /// </summary>
        public ODataFilterBuilder<T> Include(string property)
        {
            _openDataFilter.Expand =
                string.IsNullOrWhiteSpace(_openDataFilter.Expand)
                    ? property
                    : $"{_openDataFilter.Expand},{property}";

            return this;
        }

        /// <summary>
        /// Add property to the list of objects to expand in the result
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public ODataFilterBuilder<T> Include(Expression<Func<T, object>> property)
        {
            return Include(GetMemberName(property));
        }

        /// <summary>
        /// Results should include Count
        /// </summary>
        public ODataFilterBuilder<T> WithCount()
        {
            _openDataFilter.Count = true;
            return this;
        }

        /// <summary>
        /// Results should include links
        /// </summary>
        public ODataFilterBuilder<T> WithLinks()
        {
            _openDataFilter.Links = true;
            return this;
        }

        /// <summary>
        /// Constructs the IOdataFilter object
        /// </summary>
        /// <returns></returns>
        public IODataFilter BuildObject()
        {
            return _openDataFilter;
        }

        /// <summary>
        /// Constructs the query string that can be used for an API call
        /// </summary>
        public string Build()
        {
            var queryStringParameters = new List<string>();

            if (_openDataFilter.Filter != null)
                queryStringParameters.Add($"$filter={_openDataFilter.Filter}");
            if (_openDataFilter.Expand != null)
                queryStringParameters.Add($"$expand={_openDataFilter.Expand}");
            if (_openDataFilter.OrderBy != null)
                queryStringParameters.Add($"$orderBy={_openDataFilter.OrderBy}");
            if (_openDataFilter.ThenBy != null)
                queryStringParameters.Add($"thenBy={_openDataFilter.ThenBy}");
            if (_openDataFilter.Skip != null)
                queryStringParameters.Add($"$skip={_openDataFilter.Skip}");
            if (_openDataFilter.Take != null)
                queryStringParameters.Add($"$top={_openDataFilter.Take}");
            if (_openDataFilter.Count != null)
                queryStringParameters.Add($"$count={_openDataFilter.Count}");
            if (_openDataFilter.Links != null)
                queryStringParameters.Add($"$links={_openDataFilter.Links}");

            return $"?{string.Join("&", queryStringParameters)}";
        }

        private string ExpressionToString(Expression<Func<T, object>> property, Operation operation, object value)
        {
            if (value is string)
                value = $"'{value}'";
            if (value is DateTime time)
                value = time.ToString("O");

            if (operation == Operation.Contains)
            {
                return $"{operation.ToOperationString()}({GetMemberName(property)}, {value})";
            }

            return $"{GetMemberName(property)} {operation.ToOperationString()} {value}";
        }

        private static string GetMemberName(Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).Member.Name;
                case ExpressionType.Convert:
                    return GetMemberName(((UnaryExpression)expression).Operand);
                default:
                    throw new NotSupportedException(expression.NodeType.ToString());
            }
        }
    }
}
