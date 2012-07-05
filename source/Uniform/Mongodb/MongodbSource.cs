using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Uniform.Mongodb
{
    public class MongodbSource<TDocument> : IUniformable<TDocument>
    {
        private readonly IQueryable<TDocument> _queryable;
        private readonly MongodbUniformableProvider _provider;

        public IQueryable<TDocument> Source
        {
            get { return _queryable; }
        }

        public IUniformableProvider Provider
        {
            get { return _provider; }
        }

        public MongodbSource(IQueryable<TDocument> queryable)
        {
            _queryable = queryable;
            _provider = new MongodbUniformableProvider();
        }

        public IEnumerator<TDocument> GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}