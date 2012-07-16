using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Uniform.Mongodb
{
    public class MongodbCollection : ICollection
    {
        private readonly MongodbDatabase _database;
        private readonly Type _documentType;
        private readonly string _collectionName;
        private readonly MongoCollection _collection;

        public MongodbCollection(MongodbDatabase database, Type documentType, String collectionName)
        {
            if (collectionName == null) throw new ArgumentNullException("collectionName");

            _database = database;
            _documentType = documentType;
            _collectionName = collectionName;

            var mongoSettings = _database.Database.CreateCollectionSettings(documentType, _collectionName);
            mongoSettings.AssignIdOnInsert = false;
            _collection = _database.Database.GetCollection(mongoSettings);
        }

        /// <summary>
        /// Returns document by it's key. 
        /// If document doesn't exists - default(TDocument) will be returned.
        /// </summary>
        public Object GetById(String key)
        {
            if (key == null) throw new ArgumentNullException("key");

            return _collection.FindOneByIdAs(_documentType, key);
        }

        /// <summary>
        /// Saves document to collection using specified key.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        public void Save(String key, Object document)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (document == null) throw new ArgumentNullException("document");

            _database.UniformDatabase.Metadata.SetDocumentId(document, key);
            _collection.Save(document);
        }

        /// <summary>
        /// Deletes document with specified key.
        /// If document with such key doesn't exists - no changes to collection will be made.
        /// </summary>
        public void Delete(String key)
        {
            if (key == null) throw new ArgumentNullException("key");

            _collection.Remove(Query.EQ("_id", key));
        }

        public void Save(IEnumerable<Object> docs)
        {
            var mongoInsertOptions = new MongoInsertOptions();
            mongoInsertOptions.CheckElementNames = false;
            mongoInsertOptions.SafeMode = SafeMode.True;
            _collection.InsertBatch(docs, mongoInsertOptions);
        }

        public void DropAndPrepare()
        {
            _collection.Drop();
        }
    }
}