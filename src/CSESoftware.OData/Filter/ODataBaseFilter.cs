﻿using CSESoftware.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace CSESoftware.OData.Filter
{
    public class ODataBaseFilter<TEntity> : IODataBaseFilter<TEntity> where TEntity : class, IEntity
    {
        public Expression<Func<TEntity, bool>> Filter { get; set; }
        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> DefaultOrder { get; set; }
        public List<Expression<Func<TEntity, object>>> Include { get; set; }
        public int? MaxTake { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
