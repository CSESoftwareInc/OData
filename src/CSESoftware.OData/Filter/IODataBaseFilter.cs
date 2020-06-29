using CSESoftware.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CSESoftware.OData.Filter
{
    public interface IODataBaseFilter<TEntity> where TEntity : class, IBaseEntity
    {
        Expression<Func<TEntity, bool>> Filter { get; set; }
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> DefaultOrder { get; set; }
        List<Expression<Func<TEntity, object>>> Include { get; set; }
        int? MaxTake { get; set; }
    }
}
