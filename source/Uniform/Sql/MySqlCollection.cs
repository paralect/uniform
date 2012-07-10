using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MySql.Data.MySqlClient;

namespace Uniform.Sql
{
    public class MySqlCollection<TDocument> : ICollection<TDocument>
    {
        private readonly MySqlDatabase _database;
        private readonly Dictionary<String, Type> _metadata = new Dictionary<String, Type>();
        private readonly Dictionary<String, PropertyInfo> _propertyMetadata = new Dictionary<String, PropertyInfo>();
        private readonly String _collectionName;
        private readonly String _primaryFieldName;
        private readonly PropertyInfo _primaryPropertyInfo;


        public MySqlCollection(MySqlDatabase database)
        {
            _database = database;
            _collectionName = _database.Metadata.GetCollectionName(typeof (TDocument));
            _primaryPropertyInfo = _database.Metadata.GetDocumentIdPropertyInfo(typeof (TDocument));
            _primaryFieldName = _primaryPropertyInfo.Name;
            BuildMetadata();
        }

        public TDocument GetById(string key)
        {
            var command = _database.Connection.CreateCommand();
            command.CommandText = String.Format("select * from {0} where {1} = @id", _collectionName, _primaryFieldName);
            command.Parameters.AddWithValue("@id", key);

            var doc = Activator.CreateInstance<TDocument>();
            using(var data = command.ExecuteReader())
            {
                if (!data.HasRows)
                    return default(TDocument);

                data.Read();
                

                foreach (var pair in _propertyMetadata)
                {
                    var value = data.GetValue(data.GetOrdinal(pair.Key));
                    pair.Value.SetValue(doc, value, new object[0]);
                }
            }


            return doc;
        }

        object ICollection.GetById(string key)
        {
            return GetById(key);
        }

        public void Save(String key, TDocument obj)
        {
            var command = _database.Connection.CreateCommand();
            command.CommandText = BuildInsertQuery(_collectionName);

            foreach (var pair in _propertyMetadata)
            {
                var value = pair.Value.GetValue(obj, new object[0]);
                command.Parameters.AddWithValue("@" + pair.Key, value);
            }

            command.ExecuteNonQuery();
        }

        public void Save(string key, Action<TDocument> creator)
        {
            var doc = Activator.CreateInstance<TDocument>();
            creator(doc);
            Save(key, doc);
        }

        public void Update(string key, Action<TDocument> updater)
        {
            var doc = GetById(key);
            updater(doc);

            var command = _database.Connection.CreateCommand();
            command.CommandText = BuildUpdateQuery(_collectionName);

            foreach (var pair in _propertyMetadata)
            {
                var value = pair.Value.GetValue(doc, new object[0]);
                command.Parameters.AddWithValue("@" + pair.Key, value);
            }

            command.ExecuteNonQuery();            
        }

        public void UpdateOrSave(string key, Action<TDocument> updater)
        {
            var doc = GetById(key);

            if (EqualityComparer<TDocument>.Default.Equals(doc, default(TDocument)))
                Save(key, updater);
            else 
                Update(key, updater);
        }

        public void Delete(string key)
        {
            var command = _database.Connection.CreateCommand();
            command.CommandText = String.Format("delete from {0} where {1} = @id", _collectionName, _primaryFieldName);
            command.Parameters.AddWithValue("@id", key);
            command.ExecuteNonQuery();
        }

        private String BuildUpdateQuery(String tableName)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("update {0} set ", tableName);

            var index = 0;
            foreach (var pair in _metadata)
            {
                builder.AppendFormat("{0} = @{0}", pair.Key);

                if (index < _metadata.Count - 1)
                    builder.Append(", ");

                index++;
            }

            builder.AppendFormat(" where {0} = @{0}", _primaryFieldName);

            return builder.ToString();
        }

        private String BuildInsertQuery(String tableName)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("insert into {0} (", tableName);

            var index = 0;
            foreach (var pair in _metadata)
            {
                builder.AppendFormat("{0}", pair.Key);

                if (index < _metadata.Count - 1)
                    builder.Append(", ");

                index++;
            }

            builder.Append(") values (");

            var index2 = 0;
            foreach (var pair in _metadata)
            {
                builder.AppendFormat("@{0}", pair.Key);

                if (index2 < _metadata.Count - 1)
                    builder.Append(", ");

                index2++;
            }

            builder.Append(")");

            return builder.ToString();
        }

        private void BuildMetadata()
        {
            foreach (var propertyInfo in typeof(TDocument).GetProperties())
            {
                var type = propertyInfo.PropertyType;

                if (!type.IsPrimitive && 
                        type != typeof(String) &&
                        type != typeof(DateTime)
                    )
                    continue;

                _metadata.Add(propertyInfo.Name, propertyInfo.PropertyType);
                _propertyMetadata.Add(propertyInfo.Name, propertyInfo);
            }
        }

        public void CreateTable()
        {
            var command = _database.Connection.CreateCommand();
            command.CommandText = String.Format("drop table if exists {0}", _collectionName);
            command.ExecuteNonQuery();

            var builder = new StringBuilder();

            builder.AppendFormat("create table {0} (", _collectionName);

            var index = 0;
            foreach (var pair in _propertyMetadata)
            {
                if (pair.Value == _primaryPropertyInfo)
                    builder.AppendFormat("{0} {1} not null", pair.Key, "varchar(100)");
                else 
                    builder.AppendFormat("{0} {1} null", pair.Key, _typeMap[pair.Value.PropertyType]);

                builder.Append(", ");

                index++;
            }

            builder.AppendFormat(" primary key (`{0}`)", _primaryFieldName);
            builder.Append(") ENGINE=MyISAM DEFAULT CHARSET=utf8; ");

            command.CommandText = builder.ToString();
            command.ExecuteNonQuery();
        }

        private static Dictionary<Type, String> _typeMap = new Dictionary<Type, string>
        {
            { typeof(String), "varchar(1000)" },
            { typeof(Int32), "int" },
            { typeof(Int16), "int" },
            { typeof(Double), "double" },
            { typeof(Single), "float" },
            { typeof(DateTime), "datetime" },
            { typeof(byte[]), "blob" },
        };
    }

}

/*        private String BuildUpsertQuery(String tableName)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("insert into {0} (", tableName);

            var index = 0;
            foreach (var pair in _metadata)
            {
                builder.AppendFormat("{0}", pair.Key);

                if (index < _metadata.Count - 1)
                    builder.Append(", ");

                index++;
            }

            builder.Append(") values (");

            var index2 = 0;
            foreach (var pair in _metadata)
            {
                builder.AppendFormat("@{0}", pair.Key);

                if (index2 < _metadata.Count - 1)
                    builder.Append(", ");

                index2++;
            }

            builder.Append(") ON DUPLICATE KEY UPDATE ");



            return builder.ToString();            
        }*/