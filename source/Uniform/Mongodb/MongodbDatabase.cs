using System;
using MongoDB.Driver;

namespace Uniform.Mongodb
{
    /// <summary>
    /// MongoDB database wrapper. 
    /// Each collection can contains only one type of documents.
    /// Thread-safe.
    /// </summary>
    public class MongodbDatabase : IDatabase
    {
        /// <summary>
        /// MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// Name of MongoDB database 
        /// </summary>
        private readonly string _databaseName;

        /// <summary>
        /// Database metadata, contains all document types and provides some
        /// metadata related services.
        /// </summary>
        private readonly DatabaseMetadata _metadata;

        /// <summary>
        /// Database metadata, contains all document types and provides some
        /// metadata related services.
        /// </summary>
        public DatabaseMetadata Metadata
        {
            get { return _metadata; }
        }

        /// <summary>
        /// MongoDB Server
        /// </summary>
        private MongoServer Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Get database
        /// </summary>
        public MongoDatabase Database
        {
            get { return _server.GetDatabase(_databaseName); }
        }

        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongodbDatabase(String connectionString, DatabaseMetadata metadata)
        {
            _metadata = metadata;
            _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            _server = MongoServer.Create(connectionString);
        }

        /// <summary>
        /// Gets collection with specifed name that contains documents of specified type (TDocument)
        /// Will be created, if not already exists.
        /// </summary>
        public ICollection<TDocument> GetCollection<TDocument>(String name)
        {
            return new MongodbCollection<TDocument>(this, name);
        }

        /// <summary>
        /// Gets collection that contains documents of specified type (TDocument). Will be created, if not already exists.
        /// Name of collection will be taken from [Collection] attribute, that you can put on document class.
        /// If no [Collection] attribute found - type(TDocument).Name will be used for name.
        /// </summary>
        public ICollection<TDocument> GetCollection<TDocument>()
        {
            return GetCollection<TDocument>(_metadata.GetCollectionName(typeof (TDocument)));
        }
    }
}