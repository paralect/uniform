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
    public class MongodbCollection<TDocument> : ICollection<TDocument>
    {
        private readonly MongodbDatabase _database;
        private readonly MongoCollection<TDocument> _collection;

        public MongodbCollection(MongodbDatabase database, String name)
        {
            if (name == null) throw new ArgumentNullException("name");

            _database = database;
            _collection = _database.Database.GetCollection<TDocument>(name);
        }

        /// <summary>
        /// Returns document by it's key. 
        /// If document doesn't exists - default(TDocument) will be returned.
        /// </summary>
        public TDocument GetById(String key)
        {
            if (key == null) throw new ArgumentNullException("key");

            return _collection.FindOneById(key);
        }

        /// <summary>
        /// Saves document to collection using specified key.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        public void Save(String key, TDocument document)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (EqualityComparer<TDocument>.Default.Equals(document, default(TDocument)))
                throw new ArgumentNullException("document");

            _database.Metadata.SetDocumentId(document, key);
            _collection.Save(document);
        }

        /// <summary>
        /// Saves document to collection using specified key. 
        /// 'Creator' function will be applied to automatically created document of type TDocument.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        public void Save(String key, Action<TDocument> creator)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (creator == null) throw new ArgumentNullException("creator");

            var doc = Activator.CreateInstance<TDocument>();
            creator(doc);
            Save(key, doc);
        }

        /// <summary>
        /// Updates document with specified key.
        /// If document with such key doesn't exist, update will be discarded - i.e. no changes  to collection will be made. 
        /// See UpdateOrSave() method, if you need a kind of "upsert" behaviour.
        /// </summary>
        public void Update(String key, Action<TDocument> updater)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (updater == null) throw new ArgumentNullException("updater");

            var document = GetById(key);

            // if document doesn't exists (i.e. equal to default(TDocument)), stop
            if (EqualityComparer<TDocument>.Default.Equals(document, default(TDocument)))
                return;

            updater(document);
            _database.Metadata.SetDocumentId(document, key);
            Save(key, document);
            UpdateDependentDocuments(key, document);
        }

        /// <summary>
        /// Updates document with specified key.
        /// If document with such key doesn't exists, new document will be created and 'updater' function will be applied to 
        /// this newly created document.
        /// </summary>
        public void UpdateOrSave(String key, Action<TDocument> updater)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (updater == null) throw new ArgumentNullException("updater");

            var document = GetById(key);

            // if document doesn't exists (i.e. equal to default(TDocument)), stop
            if (EqualityComparer<TDocument>.Default.Equals(document, default(TDocument)))
                document = Activator.CreateInstance<TDocument>();

            updater(document);
            _database.Metadata.SetDocumentId(document, key);
            Save(key, document);
            UpdateDependentDocuments(key, document);
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

        public void UpdateDependentDocuments(String key, TDocument doc)
        {
            var builder = new MongodbDependencyBuilder(_database.Metadata);
            var deps = _database.Metadata.GetDependents(typeof(TDocument));

            foreach (var dependent in deps)
            {
                UpdateCollection(key, doc, dependent.DependentDocumentType, builder, dependent.SourceDocumentPath);
            }            
        }

        public void UpdateCollection(String key, Object doc, Type documentType, MongodbDependencyBuilder builder, List<PropertyInfo> infos)
        {
            var collectionName = _database.Metadata.GetCollectionName(documentType);
            var col = _database.Database.GetCollection(collectionName);
            var pathToQuery = builder.PathToQuery(infos, key);
            var pathToUpdate = builder.PathToUpdate(infos, BsonDocumentWrapper.Create(doc));
            col.Update(pathToQuery, pathToUpdate, UpdateFlags.Multi, SafeMode.False);
        }
    }
}