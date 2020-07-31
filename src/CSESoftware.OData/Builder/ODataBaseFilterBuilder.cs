using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CSESoftware.Core.Entity;
using CSESoftware.OData.Filter;

namespace CSESoftware.OData.Builder
{
    public class ODataBaseFilterBuilder<TEntity> where TEntity : class, IEntity
    {
        private readonly IODataBaseFilter<TEntity> _baseFilter;

        public ODataBaseFilterBuilder()
        {
            _baseFilter = new ODataBaseFilter<TEntity>()
            {
                Include = new List<Expression<Func<TEntity, object>>>()
            };
        }

        /// <summary>
        /// Set the base filtering to be applied along with the filter specified in the IODataFilter
        /// </summary>
        public ODataBaseFilterBuilder<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            _baseFilter.Filter = expression;
            return this;
        }

        /// <summary>
        /// Set the default ordering of the data. This ordering will be overridden if ordering is specified in the IODataFilter
        /// </summary>
        public ODataBaseFilterBuilder<TEntity> DefaultOrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order)
        {
            _baseFilter.DefaultOrder = order;
            return this;
        }

        /// <summary>
        /// Set the base include properties
        /// </summary>
        public ODataBaseFilterBuilder<TEntity> Include(IEnumerable<Expression<Func<TEntity, object>>> includes)
        {
            if (includes == null) return this;

            _baseFilter.Include.AddRange(includes);
            return this;
        }

        /// <summary>
        /// Set the base include properties
        /// </summary>
        public ODataBaseFilterBuilder<TEntity> Include(Expression<Func<TEntity, object>> include)
        {
            if (include == null) return this;

            _baseFilter.Include.Add(include);
            return this;
        }

        /// <summary>
        /// Set the maximum take value allowed when querying data
        /// </summary>
        /// <param name="maxTake"></param>
        /// <returns></returns>
        public ODataBaseFilterBuilder<TEntity> WithMaxTake(int? maxTake)
        {
            _baseFilter.MaxTake = maxTake;
            return this;
        }

        /// <summary>
        /// Constructs the IOdataBaseFilter
        /// </summary>
        /// <returns></returns>
        public IODataBaseFilter<TEntity> Build()
        {
            return _baseFilter;
        }
    }
}
