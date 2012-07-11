using System;
using System.Data;
using ServiceStack.OrmLite;

namespace Uniform.AdoNet
{
    public class AdoNetCollection<TDocument> : ICollection<TDocument> where TDocument : new()
    {
        private readonly AdoNetDatabase _database;
        private IDbCommand _command;

        public AdoNetCollection(AdoNetDatabase database)
        {
            _database = database;
            _command = _database.Connection.CreateCommand();

        }

        public object GetById(string key)
        {
            return _command.GetById<TDocument>(key);
        }

        TDocument ICollection<TDocument>.GetById(string key)
        {
            return (TDocument) GetById(key);
        }

        public void Save(string key, TDocument obj)
        {
            _command.Save<TDocument>(obj);
        }

        public void Save(string key, Action<TDocument> creator)
        {
            var doc = new TDocument();
            creator(doc);
            Save(key, doc);
        }

        public void Update(string key, Action<TDocument> updater)
        {
            var doc = new TDocument();
            updater(doc);
            _command.Update(doc);
        }

        public void UpdateOrSave(string key, Action<TDocument> updater)
        {
            var doc = new TDocument();
            updater(doc);
            _command.Save(doc);
        }

        public void Delete(string key)
        {
            _command.Delete<TDocument>(key);
        }

        public void CreateTable()
        {
            _command.DropTable<TDocument>();
            _command.CreateTable<TDocument>();
        }
    }
}