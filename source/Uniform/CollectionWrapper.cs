using System;
using System.Collections.Generic;
using System.Linq;

namespace Uniform
{
    public class GenericCollection<TDocument> : ICollection<TDocument> where TDocument : new()
    {
        private readonly ICollection _collection;

        public ICollection Collection
        {
            get { return _collection; }
        }

        public GenericCollection(ICollection collection)
        {
            _collection = collection;
        }

        object ICollection.GetById(string key)
        {
            return GetById(key);
        }

        public void Save(string key, object obj)
        {
            _collection.Save(key, obj);
        }

        public void Save(IEnumerable<object> docs)
        {
            _collection.Save(docs);
        }

        public void Delete(string key)
        {
            _collection.Delete(key);
        }

        public TDocument GetById(string key)
        {
            return (TDocument) _collection.GetById(key);
        }

        public void Save(string key, TDocument obj)
        {
            _collection.Save(key, obj);
        }

        public void Save(string key, Action<TDocument> creator)
        {
            var doc = new TDocument();
            creator(doc);
            _collection.Save(key, doc);
        }

        public void Save(IEnumerable<TDocument> docs)
        {
            _collection.Save(docs.Cast<Object>());
        }

        public void Update(string key, Action<TDocument> updater)
        {
            var doc = GetById(key);

            if (doc == null)
                return;

            updater(doc);
            _collection.Save(key, doc);
        }

        public void UpdateOrSave(string key, Action<TDocument> updater)
        {
            var doc = GetById(key);

            if (doc == null)
                doc = new TDocument();

            updater(doc);
            _collection.Save(key, doc);            
        }

        public void DropAndPrepare()
        {
            _collection.DropAndPrepare();
        }
    }
}