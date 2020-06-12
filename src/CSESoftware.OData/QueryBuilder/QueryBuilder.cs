using System;
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
            _openDataFilter.Filter += $"({expression})";
            return this;
        }
        public QueryBuilder<T> Where(Expression<Func<T, object>> property, Operation operation, object value)
        {
            _openDataFilter.Filter += $"({ExpressionToString(property, operation, value)})";
            return this;
        }

        public QueryBuilder<T> OrWhere(string expression)
        {
            _openDataFilter.Filter += $"or ({expression})";
            return this;
        }

        public QueryBuilder<T> OrWhere(Expression<Func<T, object>> property, Operation operation, object value)
        {
            _openDataFilter.Filter += $"or ({ExpressionToString(property, operation, value)})";
            return this;
        }

        public QueryBuilder<T> AndWhere(string expression)
        {
            _openDataFilter.Filter += $"and ({expression})";
            return this;
        }

        public QueryBuilder<T> AndWhere(Expression<Func<T, object>> property, Operation operation, object value)
        {
            _openDataFilter.Filter += $"and ({ExpressionToString(property, operation, value)})";
            return this;
        }

        public QueryBuilder<T> WithPageSize(int pageSize)
        {
            _openDataFilter.Take = pageSize;
            return this;
        }
        public QueryBuilder<T> SkipPages(int pages)
        {
            _openDataFilter.Skip = pages;
            return this;
        }

        public QueryBuilder<T> OrderBy(string property, bool descending = false)
        {
            if (descending)
                property += " desc";

            _openDataFilter.OrderBy = property;
            return this;
        }

        public QueryBuilder<T> ThenBy(string property, bool descending = false)
        {
            if (descending)
                property += " desc";

            _openDataFilter.ThenBy = property;
            return this;
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
            _openDataFilter.Expand =
                string.IsNullOrWhiteSpace(_openDataFilter.Expand)
                    ? GetMemberName(property)
                    : $"{_openDataFilter.Expand},{GetMemberName(property)}";

            return this;
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

        public IODataFilter Build()
        {
            return _openDataFilter;
        }

        private string ExpressionToString(Expression<Func<T, object>> property, Operation operation, object value)
        {
            if (operation == Operation.Contains)
            {
                return $"{operation.ToOperationString()}({GetMemberName(property)}, '{value}')";
            }

            return $"{GetMemberName(property)} {operation.ToOperationString()} {value}";
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
