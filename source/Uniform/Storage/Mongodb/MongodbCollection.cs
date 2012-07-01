using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Uniform.Storage.Mongodb
{
    public class MongodbCollection : ICollection
    {
        private readonly string _name;
        private readonly MongodbDatabase _db;
        private MongoCollection _collection;

        public MongodbCollection(String name, MongodbDatabase db)
        {
            _name = name;
            _db = db;
            _collection = _db.Database.GetCollection(name);
        }

        public object GetById(string key)
        {
            return _collection.FindOneAs<BsonDocument>(Query.EQ("_id", key));
        }

        public void Save(string key, object obj)
        {
            _db.Helper.SetDocumentId(obj, key);
            _collection.Save(obj);
        }

        public void Update(string key, Action<object> updater)
        {
            var doc = _collection.FindOneByIdAs<BsonDocument>(key);
            updater(doc);
            Save(key, doc);
        }
    }

    public class MongodbCollection<TDocument> : ICollection<TDocument>
    {
        private readonly string _name;
        private readonly MongodbDatabase _db;
        private readonly MongodbCollection _typelessCollection;
        private readonly MongoCollection<TDocument> _collection;

        public MongodbCollection(String name, MongodbDatabase db, MongodbCollection typelessCollection)
        {
            _name = name;
            _db = db;
            _typelessCollection = typelessCollection;
            _collection = _db.Database.GetCollection<TDocument>(name);
        }

        object ICollection.GetById(string key)
        {
            return _typelessCollection.GetById(key);
        }

        public void Save(string key, TDocument obj)
        {
            _db.Helper.SetDocumentId(obj, key);
            _collection.Insert(obj);
        }

        public void Save(String key, Action<TDocument> creator)
        {
            var doc = Activator.CreateInstance<TDocument>();
            creator(doc);
            Save(key, doc);
        }

        public void Update(String key, Action<TDocument> updater)
        {
            var doc = _collection.FindOneById(key);
            updater(doc);
            Save(key, doc);
        }

        public TDocument GetById(String key)
        {
            return _collection.FindOneById(key);
        }

        public void Save(String key, Object obj)
        {
            _typelessCollection.Save(key, obj);
        }

        public void Update(String key, Action<Object> updater)
        {
            _typelessCollection.Update(key, updater);
        }
    }
}