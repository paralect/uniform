using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Uniform.Common;

namespace Uniform.Storage.InMemory
{
    public class Entry
    {
        public Object Document { get; set; }
        public Dictionary<IndexDefinition, Int32> HashByIndex { get; set; }

        public Entry()
        {
            HashByIndex = new Dictionary<IndexDefinition, int>();
        }
    }

    public class InMemoryCollection : ICollection
    {
        private readonly Dictionary<string, Entry> _documents = new Dictionary<string, Entry>();

        private readonly Dictionary<String, Dictionary<int, List<object>>> _indexes =
            new Dictionary<string, Dictionary<int, List<object>>>();

        public Dictionary<string, Entry> Documents
        {
            get { return _documents; }
        }

        public Dictionary<int, List<Object>> GetIndex(String name)
        {
            Dictionary<int, List<object>> index;
            if (!_indexes.TryGetValue(name, out index))
                _indexes[name] = index = new Dictionary<int, List<object>>();

            return index;
        }


        public Object GetById(String key)
        {
            Entry value;
            if (!_documents.TryGetValue(key, out value))
                throw new Exception("Document not available");

            return value.Document;
        }

        public void Update(String key, Action<Object> updater)
        {
            var obj = GetById(key);
            updater(obj);
            Save(key, obj);
        }

        public void Save(String key, Object obj)
        {
            Entry entry;
            if (!_documents.TryGetValue(key, out entry))
                _documents[key] = entry = new Entry();

            entry.Document = obj;
            InsureIndexes(key, entry);
        }

        private IIndexContext _indexContext = null;

        public IIndexContext IndexContext
        {
            get { return _indexContext; }
        }

        public void InsureIndexes(String key, Entry obj)
        {
            CheckForIndexDefinitions(key, obj);

            foreach (var definition in _indexContext.Definitions)
            {
                var index = GetIndex(definition.Name);

                object[] values = new object[definition.Expressions.Count];
                for (int i = 0; i < definition.Expressions.Count; i++)
                {
                    var expression = definition.Expressions[i];
                    var linq = expression as LambdaExpression;
                    values[i] = linq.Compile().DynamicInvoke(obj.Document);
                }

                var hash = HashCodeUtils.Compute(values);

                List<object> list;
                if (!index.TryGetValue(hash, out list))
                    index[hash] = list = new List<object>();

                Int32 oldHash = 0;
                var alreadyHaveIndex = obj.HashByIndex.TryGetValue(definition, out oldHash);

                if (!alreadyHaveIndex)
                {
                    list.Add(obj);
                } else
                {
                    if (oldHash != hash)
                    {
                        var entries = index[oldHash];
                        entries.Remove(obj);
                        list.Add(obj);
                    }
                }

                obj.HashByIndex[definition] = hash;                
            }
        }

        public void CheckForIndexDefinitions(String key, Entry entry)
        {
            if (_indexContext != null)
                return;

            var type = entry.Document.GetType();
            var defType = typeof (IndexContext<>).MakeGenericType(type);
            var def = (IIndexContext) Activator.CreateInstance(defType);

            var mthd = type.GetMethod("DefineIndexes");

            if (mthd != null)
            {
                mthd.Invoke(entry.Document, new object[] { def });
            }

            _indexContext = def;
        }
    }
}