using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace IndexedLinq.IndexedProvider
{
    public class IndexedProviderQueryVisitor<TDocument> : QueryModelVisitorBase
    {
        private readonly MemberExpression _currentItemExpression;
        private readonly ParameterExpression _parameterExpression;

        public List<Func<TDocument, bool>> _compiledWhereClauses = new List<Func<TDocument, bool>>();

        public List<Func<TDocument, bool>> CompiledWhereClauses
        {
            get { return _compiledWhereClauses; }
        }

        public IndexedProviderQueryVisitor(MemberExpression currentItemExpression, ParameterExpression parameterExpression)
        {
            _currentItemExpression = currentItemExpression;
            _parameterExpression = parameterExpression;
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            var wherector = Expression.Lambda<Func<TDocument, Boolean>>(whereClause.Predicate, _parameterExpression);
            var compilector = wherector.Compile();
            _compiledWhereClauses.Add(compilector);
            base.VisitWhereClause(whereClause, queryModel, index);
        }
    }
}