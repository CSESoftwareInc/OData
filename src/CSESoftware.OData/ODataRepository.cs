﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSESoftware.Core.Entity;
using CSESoftware.OData.Exceptions;
using CSESoftware.OData.Filter;
using CSESoftware.Repository;
using CSESoftware.Repository.Builder;

namespace CSESoftware.OData
{
    public class ODataRepository : IODataRepository
    {
        private readonly IReadOnlyRepository _repository;

        public ODataRepository(IReadOnlyRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get entities from the repository by the filter object
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter">query string filter</param>
        /// <param name="baseFilter">optional - applies another filter as an AND operation to restrict the results</param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetEntities<TEntity>(IODataFilter filter, IODataBaseFilter<TEntity> baseFilter = null) where TEntity : class, IBaseEntity
        {
            if ((filter.Skip != null || filter.Take != null) && string.IsNullOrWhiteSpace(filter.OrderBy))
                throw new OrderingException("You must provide $orderBy if using $skip or $top");

            if (!string.IsNullOrWhiteSpace(filter.ThenBy) && string.IsNullOrWhiteSpace(filter.OrderBy))
                throw new OrderingException("You must provide $orderBy if using $thenBy");

            var filterExpression = AndAlso(baseFilter.Filter, GenerateExpressionFilter<TEntity>(filter.Filter));
            var includeExpression = GenerateIncludeExpression<TEntity>(filter.Expand ?? "", baseFilter.Include);
            var ordering = GenerateOrderingExpression<TEntity>(filter.OrderBy, filter.ThenBy, baseFilter.DefaultOrder);
            var take = GetTake(filter.Take, baseFilter.MaxTake);

            var repositoryFilter = new QueryBuilder<TEntity>()
                .Where(filterExpression)
                .OrderBy(ordering)
                .Include(includeExpression)
                .Skip(filter.Skip)
                .Take(take)
                .Build();

            return await _repository.GetAllAsync(repositoryFilter);
        }

        /// <summary>
        /// Returns the total count of the query without pagination
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        public async Task<int> GetTotalCount<TEntity>(IODataFilter filter, IODataBaseFilter<TEntity> baseFilter = null) where TEntity : class, IBaseEntity
        {
            var expression = AndAlso(baseFilter.Filter, GenerateExpressionFilter<TEntity>(filter.Filter ?? ""));
            return await _repository.GetCountAsync(expression);
        }

        /// <summary>
        /// Combines two expressions using AND
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static Expression<Func<TEntity, bool>> AndAlso<TEntity>(
            Expression<Func<TEntity, bool>> left,
            Expression<Func<TEntity, bool>> right)
        {
            if (left == null) return right;
            if (right == null) return left;

            var parameter = Expression.Parameter(typeof(TEntity));

            var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
            var leftExpression = leftVisitor.Visit(left.Body);

            var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
            var rightExpression = rightVisitor.Visit(right.Body);

            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.AndAlso(leftExpression, rightExpression), parameter);
        }

        /// <summary>
        /// Converts string filter into expression
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static Expression<Func<TEntity, bool>> GenerateExpressionFilter<TEntity>(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return null;
            }

            filter = ConvertStringsToAppropriateFormat(filter);
            filter = ConvertDateTimeToAppropriateFormat(filter);
            filter = ConvertContainToAppropriateFormat(filter);

            // Parameterize and make lambda expression
            var entity = Expression.Parameter(typeof(TEntity), "entity");
            var filterExpression = DynamicExpressionParser.ParseLambda(new[] {entity}, null, filter, null);
            return (Expression<Func<TEntity, bool>>)filterExpression;
        }

        /// <summary>
        /// Converts strings in the query to use the correct quotation marks
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        protected static string ConvertStringsToAppropriateFormat(string filter)
        {
            filter = filter.Replace(" '", " \"");
            filter = filter.Replace(",'", ",\"");
            filter = filter.Replace("('", "(\"");
            filter = filter.Replace("')", "\")");
            if(filter.EndsWith("'"))
                filter = filter.Substring(0, filter.Length - 1) + "\"";

            return filter;
        }

        /// <summary>
        /// Converts DateTime references into the format used by Dynamic Linq
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static string ConvertDateTimeToAppropriateFormat(string filter)
        {
            foreach (var item in filter.Split(' '))
            {
                var isDate = DateTime.TryParseExact(item, "yyyy-MM-ddTHH:mm:ss.ffK", null, DateTimeStyles.AdjustToUniversal, out var date);

                if (isDate)
                {
                    filter = filter.Replace(item, $"DateTime({ date:yyyy, MM, dd, HH, mm, ss})");
                }
            }

            return filter;
        }

        /// <summary>
        /// Converts Contains operations from OData format to Dynamic Linq format
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static string ConvertContainToAppropriateFormat(string filter)
        {
            const string subStringPattern = @"contains\(.*?,";
            var regex = new Regex(subStringPattern, RegexOptions.IgnoreCase);

            var matches = regex.Matches(filter);

            foreach (var match in matches)
            {
                var matchText = match.ToString();
                var columnName = matchText
                    .Replace("contains(", "")
                    .Replace(",", "");

                filter = filter.Replace(matchText, $"{columnName}.Contains(");
            }

            return filter;
        }

        /// <summary>
        /// Convert string includes to include functions
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="includes"></param>
        /// <returns></returns>
        private static List<Expression<Func<TEntity, object>>> GenerateIncludeExpression<TEntity>(string includes, List<Expression<Func<TEntity, object>>> baseIncludes)
        {
            var includesExpressions = baseIncludes;

            if (string.IsNullOrWhiteSpace(includes)) return includesExpressions;

            var baseIncludeNames = baseIncludes.Select(GetObjectType).Select(x => x.Name).ToList();
            var entity = Expression.Parameter(typeof(TEntity), "entity");

            foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (baseIncludeNames.Contains(include)) continue;

                try
                {
                    // Get Subproperty
                    if (include.Contains("/"))
                    {
                        var property = include.Split('/')[0];
                        var subProperty = include.Split('/')[1];

                        var x = Expression.Property(entity, property);
                        var y = Expression.Property(x, subProperty);
                        includesExpressions.Add(Expression.Lambda<Func<TEntity, object>>(y, entity));
                    }
                    else
                    {
                        includesExpressions.Add(
                            Expression.Lambda<Func<TEntity, object>>(Expression.Property(entity, include), entity));
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidPropertyException($"Invalid property ({include}) on ({typeof(TEntity).Name})", e);
                }
            }
            return includesExpressions;
        }

        /// <summary>
        /// Converts string ordering into ordering function
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="thenBy"></param>
        /// <returns></returns>
        private static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GenerateOrderingExpression<TEntity>(string orderBy, string thenBy,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> defaultOrder)
        {
            if (orderBy == null) return defaultOrder;

            if (thenBy == null)
            {
                return x => x.OrderBy(orderBy);
            }

            return x => x.OrderBy(orderBy).ThenBy(thenBy);
        }

        private static int? GetTake(int? take, int? maxTake)
        {
            if (take == null) return null;
            if (maxTake == null) return take;

            return take > maxTake ? maxTake : take;
        }

        public static Type GetObjectType<T>(Expression<Func<T, object>> expression)
        {
            if ((expression.Body.NodeType == ExpressionType.Convert) ||
                (expression.Body.NodeType == ExpressionType.ConvertChecked))
            {
                var unary = expression.Body as UnaryExpression;
                if (unary != null)
                    return unary.Operand.Type;
            }
            return expression.Body.Type;
        }
    }
}
