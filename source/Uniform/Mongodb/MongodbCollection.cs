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
    public class MongodbCollection : IDocumentCollection
    {
        private readonly MongodbDatabase _database;
        private readonly DatabaseMetadata _metadata;
        private readonly Type _documentType;
        private readonly string _collectionName;
        private readonly MongoCollection _collection;

        public MongodbCollection(MongodbDatabase database, Type documentType, String collectionName)
        {
            if (collectionName == null) throw new ArgumentNullException("collectionName");

            _database = database;
            _documentType = documentType;
            _collectionName = collectionName;
            _metadata = _database.UniformDatabase.Metadata;

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

        public IEnumerable<object> GetById(IEnumerable<string> keys)
        {
            if (keys.Count() == 0)
                yield break;

            var bsonIdArray = BsonArray.Create(keys);
            MongoCursor cursor = _collection.FindAs(_documentType, Query.In("_id", bsonIdArray));
            foreach (var doc in cursor)
                yield return doc;
        }

        /// <summary>
        /// Saves document to collection using specified key.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        public bool Save(String key, Object document)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (document == null) throw new ArgumentNullException("document");

            _metadata.SetDocumentId(document, key);
            _collection.Save(_documentType, document);
            return true;
        }

        public bool Save(object obj)
        {
            var key = _metadata.GetDocumentId(obj);
            return Save(key, obj);
        }

        public bool Replace(String key, object document)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (document == null) throw new ArgumentNullException("document");

            _metadata.SetDocumentId(document, key);
            var version = _metadata.GetDocumentVersion(document);

            var updateOptions = new MongoUpdateOptions
            {
                CheckElementNames = true, Flags = UpdateFlags.None, WriteConcern = WriteConcern.Acknowledged
            };

            IMongoQuery query = Query.EQ("_id", key);

            if (version != null)
            {
                // Increment version of document
                _metadata.SetDocumentVersion(document, version.Value + 1);

                var versionProperty = _metadata.GetDocumentVersionPropertyInfo(_documentType);
                query = Query.And(query, Query.EQ(versionProperty.Name, version));
            }

            var update = Update.Replace(_documentType, document);

            var result = _collection.Update(query, update, updateOptions);
            return result.DocumentsAffected > 0;
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
            mongoInsertOptions.WriteConcern = WriteConcern.Acknowledged;
            _collection.InsertBatch(docs, mongoInsertOptions);
        }

        public void DropAndPrepare()
        {
            _collection.Drop();
        }
    }
}