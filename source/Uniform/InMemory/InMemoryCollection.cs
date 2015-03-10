using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Uniform.InMemory
{
    /// <summary>
    /// In-memory simple typed collection of documents.
    /// Collection always consists only of one type of documents.
    /// Not thread-safe.
    /// </summary>
    public class InMemoryCollection : IDocumentCollection, IInMemoryCollection
    {
        private readonly DatabaseMetadata _metadata;

        /// <summary>
        /// Main data structure that contains all documents of type TDocument, hashed by key
        /// </summary>
        private readonly ConcurrentDictionary<String, Object> _documents = new ConcurrentDictionary<String, Object>();

        /// <summary>
        /// Main data structure that contains all documents of type TDocument, hashed by key
        /// </summary>
        public ConcurrentDictionary<String, Object> Documents
        {
            get { return _documents; }
        }

        public InMemoryCollection(DatabaseMetadata metadata)
        {
            _metadata = metadata;
        }

        /// <summary>
        /// Returns document by it's key. 
        /// If document doesn't exists - default(TDocument) will be returned.
        /// </summary>
        public Object GetById(String key)
        {
            if (key == null) throw new ArgumentNullException("key");

            Object document;
            if (!_documents.TryGetValue(key, out document))
                return null;

            return document;
        }

        public IEnumerable<object> GetById(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                var doc = GetById(key);

                if (doc != null)
                    yield return doc;
            }
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
            SaveInternal(key, document);
            return true;
        }

        public bool Save(object document)
        {
            if (document == null) throw new ArgumentNullException("document");

            var key = _metadata.GetDocumentId(document);
            return Save(key, document);
        }

        public bool Replace(String key, object obj)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (obj == null) throw new ArgumentNullException("obj");

            _metadata.SetDocumentId(obj, key);
            return Save(key, obj);
        }

        /// <summary>
        /// Deletes document with specified key.
        /// If document with such key doesn't exists - no changes to collection will be made.
        /// </summary>
        public void Delete(String key)
        {
            if (key == null) throw new ArgumentNullException("key");
            object doc;
            _documents.TryRemove(key, out doc);
        }

        public void Save(IEnumerable<Object> docs)
        {
            foreach (var document in docs)
            {
                var id = _metadata.GetDocumentId(document);
                _documents[id] = document;
            }
        }

        /// <summary>
        /// Saves document, by ovewriting possible existed document.
        /// </summary>
        private void SaveInternal(String key, Object document)
        {
            _documents[key] = document;
        }

        public void DropAndPrepare()
        {
            _documents.Clear();
        }
    }
}