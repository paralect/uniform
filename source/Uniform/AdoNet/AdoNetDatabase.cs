using System;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;

namespace Uniform.AdoNet
{
    public class AdoNetDatabase : IDocumentDatabase
    {
        private readonly OrmLiteConnectionFactory _dbFactory;
        private UniformDatabase _uniformDatabase;

        public UniformDatabase UniformDatabase
        {
            get { return _uniformDatabase; }
        }

        public OrmLiteConnectionFactory DbFactory
        {
            get { return _dbFactory; }
        }

        public AdoNetDatabase(String connectionString, IOrmLiteDialectProvider dialectProvider)
        {
            _dbFactory = new OrmLiteConnectionFactory(connectionString, dialectProvider);
        }

        public void Initialize(UniformDatabase database)
        {
            _uniformDatabase = database;
        }

        public IDocumentCollection<TDocument> GetCollection<TDocument>(string name) where TDocument : new()
        {
            return new GenericCollection<TDocument>(
                GetCollection(typeof(TDocument), name));
        }

        public IDocumentCollection GetCollection(Type documentType, string name)
        {
            return new AdoNetCollection(documentType, this);
        }

        public void DropAllCollections()
        {
            using (var connection = _dbFactory.OpenDbConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "EXEC sp_msforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"";
                command.ExecuteNonQuery();

                command.CommandText = "EXEC sp_MSForEachTable \"DELETE FROM ?\"";
                command.ExecuteNonQuery();
            }
        }
    }
}