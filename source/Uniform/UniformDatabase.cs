using System;

namespace Uniform
{
    public class UniformDatabase
    {
        private readonly DatabaseMetadata _metadata;

        public DatabaseMetadata Metadata
        {
            get { return _metadata; }
        }

        private UniformDatabase(DatabaseConfiguration configuration)
        {
            _metadata = new DatabaseMetadata(configuration);
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
            return null;
        }
    }
}