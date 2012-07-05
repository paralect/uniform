using System;
using System.Linq;
using Remotion.Linq.Parsing.Structure;
using Uniform.InMemory;

namespace Uniform
{
    /// <summary>
    /// Wrapper around typeless ICollection
    /// </summary>
    public class ConcreteCollection<TDocument> : ICollection<TDocument>
    {
        private readonly ICollection _collection;

        public ICollection Collection
        {
            get { return _collection; }
        }

        public ConcreteCollection(ICollection collection)
        {
            _collection = collection;
        }

        TDocument ICollection<TDocument>.GetById(string key)
        {
            return (TDocument) _collection.GetById(key);
        }

        public void Save(string key, object obj)
        {
            _collection.Save(key, obj);
        }

        public void Save(string key, Action<TDocument> creator)
        {
            var doc = Activator.CreateInstance<TDocument>();
            creator(doc);
            Save(key, doc);
        }

        public void Update(string key, Action<object> updater)
        {
            _collection.Update(key, updater);
        }

        public void Save(string key, TDocument obj)
        {
            _collection.Save(key, obj);
        }

        public void Update(string key, Action<TDocument> updater)
        {
            _collection.Update(key, obj => updater((TDocument) obj));
        }

        public object GetById(string key)
        {
            return _collection.GetById(key);
        }

        public IUniformable<TDocument> AsQueryable()
        {
            return null;
            //return new InMemorySource<TDocument>();
        }
    }
}