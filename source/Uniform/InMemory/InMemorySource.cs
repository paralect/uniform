using System;
using System.Collections;
using System.Collections.Generic;
using C1.LiveLinq.Indexing;

namespace Uniform.InMemory
{
    public class InMemorySource<T> : IUniformable<T>
    {
        private readonly IIndexedSource<T> _source; 
        private readonly IUniformableProvider _provider;

        public IUniformableProvider Provider
        {
            get { return _provider; }
        }

        public IIndexedSource<T> Source
        {
            get { return _source; }
        }

        public InMemorySource(IIndexedSource<T> source)
        {
            _provider = new InMemoryUniformableProvider();
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}