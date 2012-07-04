using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        private static IEnumerable<MemberExpression> GetMemberExpressions(Expression e)
        {
            MemberExpression memberExpression;

            while ((memberExpression = e as MemberExpression) != null)
            {
                yield return memberExpression;
                e = memberExpression.Expression;
            }
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            /*
            var exp = _indexContext.Definitions[0].Expressions[0];
            var exp2 = whereClause.Predicate;

            if (exp2.NodeType == ExpressionType.Equal)
            {
                BinaryExpression expression = (BinaryExpression)exp2;
                MemberExpression left = expression.Left as MemberExpression;
                var memberType = left.Member.MemberType;


            }

            var a = GetMemberExpressions(exp).ToList();
            var b = GetMemberExpressions(exp2).ToList();

            */

            var whereExpression = Expression.Lambda<Func<TDocument, Boolean>>(whereClause.Predicate, _parameterExpression);
            var compiledWhereExpression = whereExpression.Compile();
            _compiledWhereClauses.Add(compiledWhereExpression);
            base.VisitWhereClause(whereClause, queryModel, index);
        }
    }
}