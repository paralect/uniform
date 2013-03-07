using System;
using MongoDB.Driver;

namespace Uniform.Mongodb
{
    /// <summary>
    /// MongoDB database wrapper. 
    /// Each collection can contains only one type of documents.
    /// Thread-safe.
    /// </summary>
    public class MongodbDatabase : IDocumentDatabase
    {
        /// <summary>
        /// MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// Name of MongoDB database 
        /// </summary>
        protected readonly string DatabaseName;

        private UniformDatabase _uniformDatabase;

        public UniformDatabase UniformDatabase
        {
            get { return _uniformDatabase; }
        }

        /// <summary>
        /// MongoDB Server
        /// </summary>
        protected MongoServer Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Get database
        /// </summary>
        public virtual MongoDatabase Database
        {
            get { return _server.GetDatabase(DatabaseName); }
        }

        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongodbDatabase(String connectionString)
        {
            var mongoUrl = MongoUrl.Create(connectionString);
            DatabaseName = mongoUrl.DatabaseName;
            var mongoClient = new MongoClient(mongoUrl);
            _server = mongoClient.GetServer();
        }

        public void Initialize(UniformDatabase database)
        {
            _uniformDatabase = database;
        }

        /// <summary>
        /// Gets collection with specifed name that contains documents of specified type (TDocument)
        /// Will be created, if not already exists.
        /// </summary>
        public IDocumentCollection<TDocument> GetCollection<TDocument>(String name) where TDocument : new()
        {
            return new GenericCollection<TDocument>(GetCollection(typeof(TDocument), name));
        }

        public IDocumentCollection GetCollection(Type documentType, string name)
        {
            return new MongodbCollection(this, documentType, name);
        }

        public void DropAllCollections()
        {

        }
    }
}