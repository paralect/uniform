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
        /// Contains database collections, hashed by collection name
        /// </summary>
        private readonly Dictionary<CollectionInfo, ICollection> _collections = new Dictionary<CollectionInfo, ICollection>();

        private UniformDatabase _uniformDatabase;

        public UniformDatabase UniformDatabase
        {
            get { return _uniformDatabase; }
        }

        /// <summary>
        /// Contains database collections, hashed by collection name
        /// </summary>
        public Dictionary<CollectionInfo, ICollection> Collections
        {
            get { return _collections; }
        }


        /// <summary>
        /// Creates in-memory database with specified metadata
        /// </summary>
        public InMemoryDatabase()
        {
        }

        public void Initialize(UniformDatabase database)
        {
            _uniformDatabase = database;
        }

        /// <summary>
        /// Gets collection with specifed name that contains documents of specified type (TDocument)
        /// Will be created, if not already exists.
        /// </summary>
        public ICollection<TDocument> GetCollection<TDocument>(string name) where TDocument : new()
        {
            return new GenericCollection<TDocument>(GetCollection(typeof (TDocument), name));
        }

        public ICollection GetCollection(Type documentType, string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            var info = new CollectionInfo(documentType, name);

            ICollection collection;
            if (!_collections.TryGetValue(info, out collection))
                _collections[info] = collection = new InMemoryCollection(_uniformDatabase.Metadata);

            return collection;            
        }
    }
}