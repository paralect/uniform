using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace IndexedLinq.IndexedProvider
{
    public class IndexedProviderQueryable<T> : QueryableBase<T>
    {
        public IndexedProviderQueryable(IQueryParser queryParser, IQueryExecutor executor)
            : base(new DefaultQueryProvider(typeof(IndexedProviderQueryable<>), queryParser, executor))
        {
            
        }

        public IndexedProviderQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }
}