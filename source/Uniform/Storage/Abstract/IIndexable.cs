using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Uniform.Storage
{
    public interface IIndexable<TDocument>
    {
        void DefineIndexes(IndexContext<TDocument> definition);
    }

    public class IndexDefinition
    {
        public List<Expression> Expressions { get; set; }
        public String Name { get; set; }        
    }

    public interface IIndexContext
    {
        List<IndexDefinition> Definitions { get; }
    }

    public class IndexContext<TDocument> : IIndexContext
    {
        public List<IndexDefinition> Definitions { get; set; }

        public IndexContext()
        {
            Definitions = new List<IndexDefinition>();
        }

        public void Define<T>(String name, params Expression<Func<TDocument, T>>[] definitions)
        {
            var def = new IndexDefinition();
            def.Name = name;
            def.Expressions = ((Expression[])definitions).ToList();
            Definitions.Add(def);
        }
    }
}