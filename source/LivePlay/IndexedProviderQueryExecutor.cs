using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using C1.LiveLinq.Collections;
using C1.LiveLinq;
using C1.LiveLinq.Indexing;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Clauses.StreamedData;

namespace LivePlay
{
    public class IndexedProviderQueryExecutor<TDocument> : IQueryExecutor
    {
        private readonly IndexedCollection<TDocument> _collection;

        public IndexedProviderQueryExecutor(IndexedCollection<TDocument> collection)
        {
            _collection = collection;
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var currentItemProperty = Expression.Parameter(typeof(TDocument));
            IIndexedSource<TDocument> collection = _collection;

            // Create an expression that returns the current item when invoked.
            ParameterExpression currentItemExpression = Expression.Parameter(typeof (TDocument));


            // Now replace references like the "i" in "select i" that refers to the "i" in "from i in items"
            var mapping = new QuerySourceMapping();
            mapping.AddMapping(queryModel.MainFromClause, currentItemExpression);
            queryModel.TransformExpressions(e => ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences(e, mapping, true));


            if (queryModel.BodyClauses != null)
            {
                foreach (var bodyClause in queryModel.BodyClauses)
                {
                    var whereClause = bodyClause as WhereClause;

                    if (whereClause != null)
                    {
                        var whereExpression = Expression.Lambda<Func<TDocument, Boolean>>(whereClause.Predicate, currentItemProperty);
                        collection = _collection.Where(whereExpression);
                    }
                }
            }
            
            var selector = Expression.Lambda<Func<TDocument, T>>(queryModel.SelectClause.Selector, currentItemProperty);

            return collection.Select(selector);


//            return collection.Select(selector);
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            var sequence = ExecuteCollection<T>(queryModel);

            return returnDefaultWhenEmpty ? sequence.SingleOrDefault() : sequence.Single();
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            if (queryModel.ResultOperators.Count > 1)
                throw new Exception("Multiple result operators not supported");

            var documents = ExecuteCollection<TDocument>(queryModel);
            ResultOperatorBase resultOperator = queryModel.ResultOperators[0];

            var info = new StreamedSequenceInfo(typeof (IEnumerable<TDocument>), queryModel.SelectClause.Selector);
            var result = resultOperator.ExecuteInMemory(new StreamedSequence(documents, info));

            return (T) result.Value;
        }
    }
}