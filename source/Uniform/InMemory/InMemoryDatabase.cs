using System;
using System.Collections.Generic;

namespace Uniform.InMemory
{
    /// <summary>
    /// In-memory simple database, that contains documents, organized in collections. 
    /// Each collection can contains only one type of documents.
    /// Not thread-safe.
    /// </summary>
    public class InMemoryDatabase : IDatabase
    {
        /// <summary>
        /// Database metadata, contains all document types and provides some
        /// metadata related services.
        /// </summary>
        private readonly DatabaseMetadata _metadata;

        /// <summary>
        /// Contains database collections, hashed by collection name
        /// </summary>
        private readonly Dictionary<String, ICollection> _collections = new Dictionary<String, ICollection>();

        /// <summary>
        /// Contains database collections, hashed by collection name
        /// </summary>
        public Dictionary<String, ICollection> Collections
        {
            get { return _collections; }
        }

        /// <summary>
        /// Database metadata, contains all document types and provides some
        /// metadata related services.
        /// </summary>
        public DatabaseMetadata Metadata
        {
            get { return _metadata; }
        }

        /// <summary>
        /// Creates in-memory database with specified metadata
        /// </summary>
        public InMemoryDatabase(DatabaseMetadata metadata)
        {
            _metadata = metadata;
        }

        /// <summary>
        /// Gets collection with specifed name that contains documents of specified type (TDocument)
        /// Will be created, if not already exists.
        /// </summary>
        public ICollection<TDocument> GetCollection<TDocument>(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            ICollection collection;
            if (!_collections.TryGetValue(name, out collection))
                _collections[name] = collection = new InMemoryCollection<TDocument>();

            return (InMemoryCollection<TDocument>) collection;
        }

        /// <summary>
        /// Gets collection that contains documents of specified type (TDocument). Will be created, if not already exists.
        /// Name of collection will be taken from [Collection] attribute, that you can put on document class.
        /// If no [Collection] attribute found - type(TDocument).Name will be used for name.
        /// </summary>
        public ICollection<TDocument> GetCollection<TDocument>()
        {
            return GetCollection<TDocument>(_metadata.GetCollectionName(typeof(TDocument)));
        }
    }
}