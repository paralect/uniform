using System;
using System.Collections.Generic;

namespace Uniform
{
    public interface IDatabase
    {
        ICollection GetCollection(String name);
        ICollection<TDocument> GetCollection<TDocument>(String name);
    }

    public interface ICollection
    {
        Object GetById(String key);
        void Save(String key, Object obj);
        void Update(String key, Action<Object> updater);
    }

    public interface ICollection<TDocument> : ICollection
    {
        new TDocument GetById(String key);
        void Save(String key, TDocument obj);
        void Save(String key, Action<TDocument> creator);
        void Update(String key, Action<TDocument> updater);
    }

    public class Database : IDatabase
    {
        private readonly Dictionary<String, Collection> _collections = new Dictionary<string, Collection>();

        public ICollection GetCollection(String name)
        {
            Collection value;
            if (!_collections.TryGetValue(name, out value))
                _collections[name] = value = new Collection();

            return value;
        }

        public ICollection<TDocument> GetCollection<TDocument>(string name)
        {
            return new ConcreteCollection<TDocument>(GetCollection(name));
        }
    }

    public class Collection : ICollection
    {
        private readonly Dictionary<String, Object> _documents = new Dictionary<string, object>();

        public Object GetById(String key)
        {
            Object value;
            if (!_documents.TryGetValue(key, out value))
                throw new Exception("Document not available");

            return value;
        }

        public void Update(String key, Action<Object> updater)
        {
            var obj = GetById(key);
            updater(obj);
            Save(key, obj);
        }

        public void Save(String key, Object obj)
        {
            _documents[key] = obj;
        }
    }

    /// <summary>
    /// Wrapper around typeless ICollection
    /// </summary>
    public class ConcreteCollection<TDocument> : ICollection<TDocument>
    {
        private readonly ICollection _collection;

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
    }
}