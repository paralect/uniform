using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace Uniform.Storage.InMemory
{
    public class IndexedProviderQueryVisitor<TDocument> : QueryModelVisitorBase
    {
        private readonly IIndexContext _indexContext;
        private readonly MemberExpression _currentItemExpression;
        private readonly ParameterExpression _parameterExpression;

        public List<Func<TDocument, bool>> _compiledWhereClauses = new List<Func<TDocument, bool>>();

        public List<Func<TDocument, bool>> CompiledWhereClauses
        {
            get { return _compiledWhereClauses; }
        }

        public IndexedProviderQueryVisitor(IIndexContext indexContext, MemberExpression currentItemExpression, ParameterExpression parameterExpression)
        {
            _indexContext = indexContext;
            _currentItemExpression = currentItemExpression;
            _parameterExpression = parameterExpression;
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {


            var whereExpression = Expression.Lambda<Func<TDocument, Boolean>>(whereClause.Predicate, _parameterExpression);
            var compiledWhereExpression = whereExpression.Compile();
            _compiledWhereClauses.Add(compiledWhereExpression);
            base.VisitWhereClause(whereClause, queryModel, index);
        }
    }
}