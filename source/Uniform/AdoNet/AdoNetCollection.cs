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

        public AdoNetCollection(Type documentType, AdoNetDatabase database)
        {
            _documentType = documentType;
            _database = database;
            _command = _database.Connection.CreateCommand();

        }

        public object GetById(string key)
        {
            return _command.GetByIdOrDefault(_documentType, key);
        }

        public void Save(string key, Object obj)
        {
            _command.Save(_documentType, obj, _database.UniformDatabase.Metadata.GetDocumentId(obj));
        }

        public void Delete(string key)
        {
            _command.DeleteById(_documentType, key);
        }

/*        public void CreateTable()
        {
            _command.DropTable<TDocument>();
            _command.CreateTable<TDocument>();
        }*/

        public void Save(IEnumerable<Object> docs)
        {
            using (var connection = _database.DbFactory.OpenDbConnection())
            {
                using(var transaction = _database.Connection.BeginTransaction())
                {
                    _command.InsertAll(docs);
                    transaction.Commit();
                }
            }
        }
    }
}