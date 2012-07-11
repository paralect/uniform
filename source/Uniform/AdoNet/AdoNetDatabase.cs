using System;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;

namespace Uniform.AdoNet
{
    public class AdoNetDatabase : IDatabase
    {
        private readonly DatabaseMetadata _metadata;
        private readonly OrmLiteConnectionFactory _dbFactory;
        private IDbConnection _connection;

        public IDbConnection Connection
        {
            get { return _connection; }
        }

        public AdoNetDatabase(String connectionString, DatabaseMetadata metadata)
        {
            _metadata = metadata;
            _dbFactory = new OrmLiteConnectionFactory(connectionString, MySqlDialectProvider.Instance);
            _connection = _dbFactory.OpenDbConnection();
        }

        public DatabaseMetadata Metadata
        {
            get { return _metadata; }
        }

        public ICollection<TDocument> GetCollection<TDocument>(string name) where TDocument : new()
        {
            return new AdoNetCollection<TDocument>(this);
        }

        public ICollection<TDocument> GetCollection<TDocument>() where TDocument : new()
        {
            return GetCollection<TDocument>("");
        }
    }
}