using System;
using System.Collections.Generic;
using System.Data;
using ServiceStack.OrmLite;

namespace Uniform.AdoNet
{
    public class AdoNetCollection : IDocumentCollection
    {
        private readonly Type _documentType;
        private readonly AdoNetDatabase _database;
        private readonly IDbConnectionFactory _factory;

        public AdoNetCollection(Type documentType, AdoNetDatabase database)
        {
            _documentType = documentType;
            _database = database;
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

        public bool Save(string key, Object obj)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (obj == null) throw new ArgumentNullException("obj");

            using (var connection = _factory.OpenDbConnection())
            {
                var command = connection.CreateCommand();
                command.Save(_documentType, obj, key);
            }

            return true;
        }

        public bool Save(Object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var key = _database.UniformDatabase.Metadata.GetDocumentId(obj);
            return Save(key, obj);
        }

        public bool Replace(String key, object obj)
        {
            return Save(key, obj);
        }

        public void Delete(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            using (var connection = _factory.OpenDbConnection())
            {
                var command = connection.CreateCommand();
                command.DeleteById(_documentType, key);
            }
        }

        public void Save(IEnumerable<Object> docs)
        {
            using (var connection = _factory.OpenDbConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.InsertAll(docs);
                transaction.Commit();
            }
        }

        public void DropAndPrepare()
        {
            using (var connection = _factory.OpenDbConnection())
            {
                var command = connection.CreateCommand();
                command.CreateTable(true, _documentType, false);
            }
        }
    }
}