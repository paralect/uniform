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
    public class InMemoryCollection : ICollection, IInMemoryCollection
    {
        private readonly DatabaseMetadata _metadata;

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

        /// <summary>
        /// Saves document to collection using specified key.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        public void Save(String key, Object document)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (document == null) throw new ArgumentNullException("document");

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