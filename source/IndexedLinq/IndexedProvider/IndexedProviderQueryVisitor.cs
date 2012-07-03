using System;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace IndexedLinq.IndexedProvider
{
    public class IndexedProviderQueryVisitor : QueryModelVisitorBase
    {
        public IndexedProviderQueryVisitor(Expression currentItemExpression, ParameterExpression parameterExpression)
        {
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            /*            if ()

                        var wherector = Expression.Lambda<Func<SampleDataSourceItem, Boolean>>(whereClause.Predicate, currentItemProperty);
                        var compilector = wherector.Compile();

                        base.VisitWhereClause(whereClause, queryModel, index);*/
        }
    }
}