using System;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;

namespace Uniform.AdoNet
{
    public class AdoNetDatabase : IDatabase
    {
        private readonly OrmLiteConnectionFactory _dbFactory;
        private IDbConnection _connection;
        private UniformDatabase _uniformDatabase;

        public UniformDatabase UniformDatabase
        {
            get { return _uniformDatabase; }
        }

        public IDbConnection Connection
        {
            get { return _connection; }
        }

        public OrmLiteConnectionFactory DbFactory
        {
            get { return _dbFactory; }
        }

        public AdoNetDatabase(String connectionString)
        {
            _dbFactory = new OrmLiteConnectionFactory(connectionString, MySqlDialectProvider.Instance);
            _connection = _dbFactory.OpenDbConnection();
        }

        public void Initialize(UniformDatabase database)
        {
            _uniformDatabase = database;
        }

        public ICollection<TDocument> GetCollection<TDocument>(string name) where TDocument : new()
        {
            return new GenericCollection<TDocument>(
                new AdoNetCollection(typeof(TDocument), this));
        }

        public ICollection GetCollection(string name)
        {
            throw new NotImplementedException();
        }
    }
}