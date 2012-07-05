using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using C1.LiveLinq.Collections;
using Uniform.Utils;

namespace Uniform.InMemory
{
    public class InMemoryCollection<TDocument> : ICollection<TDocument>
    {
        private readonly IndexedCollection<TDocument> _indexed = new IndexedCollection<TDocument>();
        private readonly Dictionary<string, TDocument> _documents = new Dictionary<string, TDocument>();

        Object ICollection.GetById(String key)
        {
            TDocument value;
            if (!_documents.TryGetValue(key, out value))
                throw new Exception("Document not available");

            return value;
        }

        public TDocument GetById(string key)
        {
            return (TDocument) ((ICollection)this).GetById(key);
        }

        public void Update(String key, Action<Object> updater)
        {
            var obj = GetById(key);
            updater(obj);
            Save(key, obj);
        }

        public void Delete(string key)
        {
            var obj = GetById(key);
            _documents.Remove(key);
            _indexed.Remove(obj);
        }

        public void Save(string key, Action<TDocument> creator)
        {
            var doc = Activator.CreateInstance<TDocument>();
            creator(doc);
            Save(key, doc);            
        }

        public void Update(string key, Action<TDocument> updater)
        {
            var obj = GetById(key);
            updater(obj);
            Save(key, obj);
        }

        public IUniformable<TDocument> AsQueryable()
        {
            return new InMemorySource<TDocument>(_indexed);
        }

        public void Save(String key, Object obj)
        {
            _documents[key] = (TDocument) obj;
            _indexed.Add((TDocument) obj);
        }

        public void Save(string key, TDocument obj)
        {
            _documents[key] = obj;
            _indexed.Add(obj);
        }
    }
}