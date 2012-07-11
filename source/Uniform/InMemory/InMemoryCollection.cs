using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Uniform.Utils;

namespace Uniform.InMemory
{
    /// <summary>
    /// In-memory simple typed collection of documents.
    /// Collection always consists only of one type of documents.
    /// Not thread-safe.
    /// </summary>
    public class InMemoryCollection<TDocument> : ICollection<TDocument>, IInMemoryCollection where TDocument : new()
    {
        /// <summary>
        /// Main data structure that contains all documents of type TDocument, hashed by key
        /// </summary>
        private readonly Dictionary<String, Object> _documents = new Dictionary<String, Object>();

        /// <summary>
        /// Main data structure that contains all documents of type TDocument, hashed by key
        /// </summary>
        public Dictionary<String, Object> Documents
        {
            get { return _documents; }
        }

        /// <summary>
        /// Returns document by it's key. 
        /// If document doesn't exists - default(TDocument) will be returned.
        /// </summary>
        public TDocument GetById(String key)
        {
            if (key == null) throw new ArgumentNullException("key");

            Object document;
            if (!_documents.TryGetValue(key, out document))
                return default(TDocument);

            return (TDocument) document;
        }

        /// <summary>
        /// Returns document by it's key. 
        /// If document doesn't exists - default(TDocument) will be returned.
        /// </summary>
        Object ICollection.GetById(string key)
        {
            return GetById(key);
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

            SaveInternal(key, document);
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

            var document = Activator.CreateInstance<TDocument>();
            creator(document);
            SaveInternal(key, document);
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
            SaveInternal(key, document);
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
            SaveInternal(key, document);
        }

        /// <summary>
        /// Deletes document with specified key.
        /// If document with such key doesn't exists - no changes to collection will be made.
        /// </summary>
        public void Delete(String key)
        {
            if (key == null) throw new ArgumentNullException("key");
            _documents.Remove(key);
        }

        /// <summary>
        /// Saves document, by ovewriting possible existed document.
        /// </summary>
        private void SaveInternal(String key, TDocument document)
        {
            _documents[key] = document;
        }
    }
}