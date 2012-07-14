using System;

namespace Uniform
{
    public class UniformDatabase
    {
        private readonly DatabaseConfiguration _configuration;

        private UniformDatabase(DatabaseConfiguration configuration)
        {
            _configuration = configuration;
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