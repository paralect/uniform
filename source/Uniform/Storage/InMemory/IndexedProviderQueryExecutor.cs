using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IndexedLinq.IndexedProvider;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.StreamedData;

namespace Uniform.Storage.InMemory
{
    public class CurrentState<T>
    {
        public T Current { get; set; }
    }

    public class IndexedProviderQueryExecutor<TDocument> : IQueryExecutor
    {
        private readonly ConcreteCollection<TDocument> _collection;
        private readonly Dictionary<String, Object> _documents; 

        public IndexedProviderQueryExecutor(ConcreteCollection<TDocument> collection)
        {
            _collection = collection;
            _documents = ((InMemoryCollection) collection.Collection).Documents;
        }

//        // Set up a proeprty that will hold the current item being enumerated.
//        public SampleDataSourceItem Current { get; private set; }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var state = new CurrentState<TDocument>() { Current = default(TDocument) };

            // Create an expression that returns the current item when invoked.
            MemberExpression currentItemExpression = Expression.Property(Expression.Constant(state), "Current");

            // Now replace references like the "i" in "select i" that refers to the "i" in "from i in items"
            var mapping = new QuerySourceMapping();
            mapping.AddMapping(queryModel.MainFromClause, currentItemExpression);
            queryModel.TransformExpressions(e => ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(e, mapping, true));

            // Create a lambda that takes our SampleDataSourceItem and passes it through the select clause
            // to produce a type of T.  (T may be SampleDataSourceItem, in which case this is an identity function.)
            var currentItemProperty = Expression.Parameter(typeof(TDocument));
            var projection = Expression.Lambda<Func<TDocument, T>>(queryModel.SelectClause.Selector, currentItemProperty);
            var projector = projection.Compile();

            var visitor = new IndexedProviderQueryVisitor<TDocument>(currentItemExpression, currentItemProperty);

            
            for (int i = 0; i < queryModel.BodyClauses.Count; i++)
            {
                var bodyClause = queryModel.BodyClauses[i];
                bodyClause.Accept(visitor, queryModel, i);
            }

            // Pretend we're getting SampleDataSourceItems from somewhere...

            foreach (var document in _documents)
            {
                var doc = (TDocument) document.Value;
                state.Current = doc;

                if (visitor.CompiledWhereClauses.Count >= 0)
                {
                    bool satisfy = true;
                    foreach (var compiledWhereClause in visitor.CompiledWhereClauses)
                    {
                        if (!compiledWhereClause(doc))
                        {
                            satisfy = false;
                            break;
                        }
                    }

                    if (!satisfy)
                        continue;
                }

                yield return projector(doc);
            }

        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var sequence = ExecuteCollection<T>(queryModel);

            return returnDefaultWhenEmpty ? sequence.SingleOrDefault() : sequence.Single();
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            var res = ExecuteCollection<TDocument>(queryModel);

            ResultOperatorBase aga = queryModel.ResultOperators[0];

            var info = new StreamedSequenceInfo(typeof (IEnumerable<TDocument>), queryModel.SelectClause.Selector);

            var result = aga.ExecuteInMemory(new StreamedSequence(res, info));

            return (T) result.Value;
        }
    }
}