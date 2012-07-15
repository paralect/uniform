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

        private UniformDatabase _uniformDatabase;

        public UniformDatabase UniformDatabase
        {
            get { return _uniformDatabase; }
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
        public MongodbDatabase(String connectionString)
        {
            _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            _server = MongoServer.Create(connectionString);
        }

        public void Initialize(UniformDatabase database)
        {
            _uniformDatabase = database;
        }

        /// <summary>
        /// Gets collection with specifed name that contains documents of specified type (TDocument)
        /// Will be created, if not already exists.
        /// </summary>
        public ICollection<TDocument> GetCollection<TDocument>(String name) where TDocument : new()
        {
            return new GenericCollection<TDocument>(
                new MongodbCollection(this, typeof(TDocument), name));
        }

        public ICollection GetCollection(string name)
        {
            throw new NotImplementedException();
        }
    }
}