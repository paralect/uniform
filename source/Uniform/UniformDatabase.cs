using System;
using System.Collections.Generic;
using Uniform.InMemory;

namespace Uniform
{
    public class UniformDatabase
    {
        private Boolean _inMemory = false;
        private Dictionary<String, IDatabase> _databases = new Dictionary<string, IDatabase>();

        private readonly DatabaseMetadata _metadata;

        public DatabaseMetadata Metadata
        {
            get { return _metadata; }
        }

        private UniformDatabase(DatabaseConfiguration configuration)
        {
            _metadata = new DatabaseMetadata(configuration);

            // Initialize all databases
            foreach (var database in _metadata.Databases)
                database.Value.Initialize(this);
        }

        /// <summary>
        /// Factory method, that creates DatabaseMetadata
        /// </summary>
        public static UniformDatabase Create(Action<DatabaseConfiguration> configurator)
        {
            var configuration = new DatabaseConfiguration();
            configurator(configuration);
            var database = new UniformDatabase(configuration);
            return database;
        }

        public ICollection<TDocument> GetCollection<TDocument>(String databaseName, String collectionName) where TDocument : new()
        {
            return _metadata.Databases[databaseName].GetCollection<TDocument>(collectionName);
        }

        public void EnterInMemoryMode()
        {
            _inMemory = true;
            _databases = _metadata.Databases;

            var newDatabases = new Dictionary<String, IDatabase>();

            foreach (var database in _metadata.Databases)
                newDatabases[database.Key] = new InMemoryDatabase();

            _metadata.Databases = newDatabases;
        }

        public void LeaveInMemoryMode(bool flush = false)
        {
            _inMemory = false;
            _metadata.Databases = _databases;

            if (flush)
                Flush();
        }

        private void Flush()
        {
            
        }
    }
}