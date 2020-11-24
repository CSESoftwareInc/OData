using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CSESoftware.Core.Entity;
using CSESoftware.OData.Filter;

namespace CSESoftware.OData
{
    public interface IODataRepository
    {
        Task<List<TEntity>> GetEntities<TEntity>(IODataFilter filter, IODataBaseFilter<TEntity> baseFilter = null)
            where TEntity : class, IEntity;

        Task<List<TOut>> GetEntities<TEntity, TOut>(IODataFilter filter, Expression<Func<TEntity, TOut>> select, IODataBaseFilter<TEntity> baseFilter = null)
            where TEntity : class, IEntity;

        Task<int> GetTotalCount<TEntity>(IODataFilter filter, IODataBaseFilter<TEntity> baseFilter = null)
            where TEntity : class, IEntity;
    }
}
