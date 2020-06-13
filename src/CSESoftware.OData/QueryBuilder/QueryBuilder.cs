using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CSESoftware.OData.QueryBuilder
{
    public class QueryBuilder<T>
    {
        private readonly IODataFilter _openDataFilter;

        public QueryBuilder()
        {
            _openDataFilter = new ODataFilter();
        }

        public QueryBuilder<T> Where(string expression) //todo value range? //todo comments
        {
            _openDataFilter.Filter = $"({expression})";
            return this;
        }
        public QueryBuilder<T> Where(Expression<Func<T, object>> property, Operation operation, object value)
        {
            return Where(ExpressionToString(property, operation, value));
        }

        public QueryBuilder<T> OrWhere(string expression)
        {
            _openDataFilter.Filter += $"or ({expression})";
            return this;
        }

        public QueryBuilder<T> OrWhere(Expression<Func<T, object>> property, Operation operation, object value)
        {
            return Where(ExpressionToString(property, operation, value));
        }

        public QueryBuilder<T> AndWhere(string expression)
        {
            _openDataFilter.Filter += $"and ({expression})";
            return this;
        }

        public QueryBuilder<T> AndWhere(Expression<Func<T, object>> property, Operation operation, object value)
        {
            return Where(ExpressionToString(property, operation, value));
        }

        public QueryBuilder<T> WhereBetween(Expression<Func<T, object>> property, object lowerBound, object upperBound)
		{
            _openDataFilter.Filter = $"({ExpressionToString(property, Operation.GreaterThanOrEqualTo, lowerBound)} " +
                $"and {ExpressionToString(property, Operation.LessThanOrEqualTo, upperBound)})";
            return this;
        }
        public QueryBuilder<T> WhereExcusiveBetween(Expression<Func<T, object>> property, object lowerBound, object upperBound)
        {
            _openDataFilter.Filter = $"({ExpressionToString(property, Operation.GreaterThan, lowerBound)} " +
                $"and {ExpressionToString(property, Operation.LessThan, upperBound)})";
            return this;
        }

        public QueryBuilder<T> Take(int take)
        {
            _openDataFilter.Take = take;
            return this;
        }
        public QueryBuilder<T> Skip(int skip)
        {
            _openDataFilter.Skip = skip;
            return this;
        }

        public QueryBuilder<T> OrderBy(string property, bool descending = false)
        {
            if (descending)
                property += " desc";

            _openDataFilter.OrderBy = property;
            return this;
        }

        public QueryBuilder<T> OrderBy(Expression<Func<T, object>> property, bool descending = false)
		{
            return OrderBy(GetMemberName(property), descending);
		}

        public QueryBuilder<T> ThenBy(string property, bool descending = false)
        {
            if (descending)
                property += " desc";

            _openDataFilter.ThenBy = property;
            return this;
        }

        public QueryBuilder<T> ThenBy(Expression<Func<T, object>> property, bool descending = false)
		{
            return ThenBy(GetMemberName(property), descending);
        }

        public QueryBuilder<T> Include(string property)
        {
            _openDataFilter.Expand =
                string.IsNullOrWhiteSpace(_openDataFilter.Expand)
                    ? property
                    : $"{_openDataFilter.Expand},{property}";

            return this;
        }

        public QueryBuilder<T> Include(Expression<Func<T, object>> property)
        {
            return Include(GetMemberName(property));
        }

        public QueryBuilder<T> WithCount()
        {
            _openDataFilter.Count = true;
            return this;
        }

        public QueryBuilder<T> WithLinks()
        {
            _openDataFilter.Links = true;
            return this;
        }

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
