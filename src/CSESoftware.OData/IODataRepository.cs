using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CSESoftware.Core.Entity;

namespace CSESoftware.OData
{
    public interface IODataRepository
    {
        Task<IEnumerable<TEntity>> GetEntities<TEntity>(IODataFilter filter, Expression<Func<TEntity, bool>> baseFilter = null)
            where TEntity : class, IBaseEntity;

        Task<int> GetTotalCount<TEntity>(IODataFilter filter, Expression<Func<TEntity, bool>> baseFilter = null)
            where TEntity : class, IBaseEntity;
    }
}