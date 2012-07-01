using System;
using MongoDB.Driver;

namespace Uniform.Storage.Mongodb
{
    public class MongodbDatabase : IDatabase
    {
        private DocumentHelper _helper = new DocumentHelper();

        public DocumentHelper Helper
        {
            get { return _helper; }
        }

        /// <summary>
        /// MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// Name of database 
        /// </summary>
        private readonly string _databaseName;

        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongodbDatabase(String connectionString)
        {
            _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            _server = MongoServer.Create(connectionString);
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

        public ICollection GetCollection(string name)
        {
            return new MongodbCollection(name, this);
        }

        public ICollection<TDocument> GetCollection<TDocument>(string name)
        {
            return new MongodbCollection<TDocument>(name, this, new MongodbCollection(name, this));
        }
    }
}