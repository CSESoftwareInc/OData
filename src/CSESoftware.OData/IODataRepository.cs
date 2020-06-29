using System.Collections.Generic;
using System.Threading.Tasks;
using CSESoftware.Core.Entity;
using CSESoftware.OData.Filter;

namespace CSESoftware.OData
{
    public interface IODataRepository
    {
        Task<IEnumerable<TEntity>> GetEntities<TEntity>(IODataFilter filter, IODataBaseFilter<TEntity> baseFilter = null)
            where TEntity : class, IBaseEntity;

        Task<int> GetTotalCount<TEntity>(IODataFilter filter, IODataBaseFilter<TEntity> baseFilter = null)
            where TEntity : class, IBaseEntity;
    }
}