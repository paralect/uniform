using System;
using MySql.Data.MySqlClient;

namespace Uniform.Sql
{
    public class MySqlDatabase : IDatabase
    {
        private readonly DatabaseMetadata _metadata;
        private readonly MySqlConnection _connection;

        private String _connectionString; 

        public MySqlDatabase(String connectionString, DatabaseMetadata metadata)
        {
            _metadata = metadata;
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        }

        public MySqlConnection Connection
        {
            get { return _connection; }
        }

        public DatabaseMetadata Metadata
        {
            get { return _metadata; }
        }

        public ICollection<TDocument> GetCollection<TDocument>(string name) where TDocument : new()
        {
            return new MySqlCollection<TDocument>(this);
        }

        public ICollection<TDocument> GetCollection<TDocument>() where TDocument : new()
        {
            return GetCollection<TDocument>(_metadata.GetCollectionName(typeof(TDocument)));
        }
    }
}