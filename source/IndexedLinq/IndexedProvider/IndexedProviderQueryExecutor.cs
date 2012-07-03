using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IndexedLinq.Tests;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;

namespace IndexedLinq.IndexedProvider
{
    public class IndexedProviderQueryExecutor : IQueryExecutor
    {
        // Set up a proeprty that will hold the current item being enumerated.
        public SampleDataSourceItem Current { get; private set; }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            // Create an expression that returns the current item when invoked.
            Expression currentItemExpression = Expression.Property(Expression.Constant(this), "Current");

            // Now replace references like the "i" in "select i" that refers to the "i" in "from i in items"
            var mapping = new QuerySourceMapping();
            mapping.AddMapping(queryModel.MainFromClause, currentItemExpression);
            queryModel.TransformExpressions(e =>
                ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(e, mapping, true));

            // Create a lambda that takes our SampleDataSourceItem and passes it through the select clause
            // to produce a type of T.  (T may be SampleDataSourceItem, in which case this is an identity function.)
            var currentItemProperty = Expression.Parameter(typeof(SampleDataSourceItem));
            var projection = Expression.Lambda<Func<SampleDataSourceItem, T>>(queryModel.SelectClause.Selector, currentItemProperty);
            var projector = projection.Compile();

            var visitor = new IndexedProviderQueryVisitor(currentItemExpression, currentItemProperty);

            /*
            for (int i = 0; i < queryModel.BodyClauses.Count; i++)
            {
                var bodyClause = queryModel.BodyClauses[i];
                bodyClause.Accept(visitor, queryModel, i);
            }*/

            var aga = queryModel.BodyClauses[0] as WhereClause;
            var wherector = Expression.Lambda<Func<SampleDataSourceItem, Boolean>>(aga.Predicate, currentItemProperty);
            var compilector = wherector.Compile();

            // Pretend we're getting SampleDataSourceItems from somewhere...
            for (var i = 0; i < 10; i++)
            {
                // Set the current item so currentItemExpression can access it.
                Current = new SampleDataSourceItem
                {
                    Name = "Name " + i,
                    Description = "This describes the item in position " + i
                };

                if (!compilector(Current))
                    continue;

                // Use the projector to convert (if necessary) the current item to what is being selected and return it.
                yield return projector(Current);
            }
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var sequence = ExecuteCollection<T>(queryModel);

            return returnDefaultWhenEmpty ? sequence.SingleOrDefault() : sequence.Single();
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            var aga = queryModel.ResultOperators[0];
            //            aga.a
            // We'll get to this one later...
            throw new NotImplementedException();
        }
    }
}