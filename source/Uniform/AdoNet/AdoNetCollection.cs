using System;
using System.Collections.Generic;
using System.Data;
using ServiceStack.OrmLite;

namespace Uniform.AdoNet
{
    public class AdoNetCollection : ICollection
    {
        private readonly Type _documentType;
        private readonly AdoNetDatabase _database;
        private IDbCommand _command;
        private readonly IDbConnectionFactory _factory;

        public AdoNetCollection(Type documentType, AdoNetDatabase database)
        {
            _documentType = documentType;
            _database = database;
            _command = _database.Connection.CreateCommand();
            _factory = database.DbFactory;
        }

        public object GetById(string key)
        {
            using (var connection = _factory.OpenDbConnection())
            {
                var command = connection.CreateCommand();
                return command.GetByIdOrDefault(_documentType, key);
            }
        }

        public IEnumerable<object> GetById(IEnumerable<string> keys)
        {
            throw new NotImplementedException("GetById(IEnumerable<String> keys) not implemented for AdoNetCollection");
        }

        public void Save(string key, Object obj)
        {
            using (var connection = _factory.OpenDbConnection())
            {
                var command = connection.CreateCommand();
                command.Save(_documentType, obj, key);
            }
        }

        public void Save(Object obj)
        {
            var key = _database.UniformDatabase.Metadata.GetDocumentId(obj);
            Save(key, obj);
        }

        public void Delete(string key)
        {
            using (var connection = _factory.OpenDbConnection())
            {
                var command = connection.CreateCommand();
                command.DeleteById(_documentType, key);
            }
        }

/*        public void CreateTable()
        {
            _command.DropTable<TDocument>();
            _command.CreateTable<TDocument>();
        }*/

        public void Save(IEnumerable<Object> docs)
        {
            using (var connection = _factory.OpenDbConnection())
            {
                using(var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.InsertAll(docs);
                    transaction.Commit();
                }
            }
        }

        public void DropAndPrepare()
        {
            using (var connection = _factory.OpenDbConnection())
            {
                var command = connection.CreateCommand();
                command.CreateTable(true, _documentType);
            }
        }
    }
}