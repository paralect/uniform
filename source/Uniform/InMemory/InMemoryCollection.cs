using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Uniform.Utils;

namespace Uniform.InMemory
{
    public class InMemoryCollection<TDocument> : ICollection<TDocument>
    {
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

        public void Delete(string key)
        {
            _documents.Remove(key);
        }

        public void Update(String key, Action<Object> updater)
        {
            var obj = GetById(key);
            updater(obj);
            InternalSave(key, obj, true);
        }

        public void Update(string key, Action<TDocument> updater)
        {
            var obj = GetById(key);
            updater(obj);
            InternalSave(key, obj, true);
        }

        public void Save(String key, Object obj)
        {
            InternalSave(key, (TDocument) obj);
        }

        public void Save(string key, TDocument obj)
        {
            InternalSave(key, obj);
        }

        public void Save(string key, Action<TDocument> creator)
        {
            var doc = Activator.CreateInstance<TDocument>();
            creator(doc);
            InternalSave(key, doc);
        }

        private void InternalSave(String key, TDocument document, Boolean updated = false)
        {
            _documents[key] = document;
        }
    }
}