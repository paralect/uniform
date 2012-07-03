using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Uniform.Storage
{
    public interface IIndexable<TDocument>
    {
        void DefineIndexes(IndexDefinition<TDocument> definition);
    }

    public interface IIndexDefinition
    {
        List<Expression> Expressions { get; set; }
        String Name { get; set; }        
    }

    public class IndexDefinition<TDocument> : IIndexDefinition
    {
        public List<Expression> Expressions { get; set; }
        public String Name { get; set; }

        public void Define<T>(String name, params Expression<Func<TDocument, T>>[] definitions)
        {
            Expressions = ((Expression[])definitions).ToList();
        }
    }
}