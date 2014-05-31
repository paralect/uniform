using System;
using System.Collections.Generic;
using System.Linq;

namespace Uniform
{
    public class GenericCollection<TDocument> : IDocumentCollection<TDocument> where TDocument : new()
    {
        private readonly IDocumentCollection _collection;

        public IDocumentCollection Collection
        {
            get { return _collection; }
        }

        public GenericCollection(IDocumentCollection collection)
        {
            _collection = collection;
        }

        object IDocumentCollection.GetById(string key)
        {
            return GetById(key);
        }

        IEnumerable<TDocument> IDocumentCollection<TDocument>.GetById(IEnumerable<string> keys)
        {
            var docs = _collection.GetById(keys);

            foreach (var doc in docs)
                yield return (TDocument) doc;
        }

        IEnumerable<object> IDocumentCollection.GetById(IEnumerable<string> keys)
        {
            return _collection.GetById(keys);
        }

        public bool Save(string key, object obj)
        {
            return _collection.Save(key, obj);
        }

        public bool Save(object obj)
        {
            return _collection.Save(obj);
        }

        public void Save(TDocument obj)
        {
            _collection.Save(obj);
        }

        public void Save(IEnumerable<object> docs)
        {
            _collection.Save(docs);
        }

        public bool Replace(String key, object obj)
        {
            return _collection.Replace(key, obj);
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

        public void Save(Action<TDocument> creator)
        {
            var doc = new TDocument();
            creator(doc);
            _collection.Save(doc);
        }

        public void Save(IEnumerable<TDocument> docs)
        {
            _collection.Save(docs.Cast<Object>());
        }

        public void Update(string key, Action<TDocument> updater)
        {
            MakeAttempts(() =>
            {
                var doc = GetById(key);

                if (doc == null)
                    return true;

                updater(doc);
                return _collection.Replace(key, doc);
            });
        }

        public void UpdateOrSave(string key, Action<TDocument> updater)
        {
            MakeAttempts(() =>
            {
                var doc = GetById(key);

                if (doc == null)
                {
                    doc = new TDocument();
                    updater(doc);
                    return _collection.Save(key, doc);
                }

                updater(doc);
                return _collection.Replace(key, doc);
            });
        }

        private void MakeAttempts(Func<Boolean> action)
        {
            const Int32 maxAttempts = 6;
            var attempts = 0;

            while (attempts < maxAttempts)
            {
                bool saved = action();

                if (saved)
                    return;

                attempts++;
            }
        }

        public void DropAndPrepare()
        {
            _collection.DropAndPrepare();
        }
    }
}
