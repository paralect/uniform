using System;
using System.Collections.Generic;

namespace Uniform.Storage.InMemory
{
    public class InMemoryCollection : ICollection
    {
        private readonly Dictionary<string, object> _documents = new Dictionary<string, object>();

        public Object GetById(String key)
        {
            Object value;
            if (!_documents.TryGetValue(key, out value))
                throw new Exception("Document not available");

            return value;
        }

        public void Update(String key, Action<Object> updater)
        {
            var obj = GetById(key);
            updater(obj);
            Save(key, obj);
        }

        public void Save(String key, Object obj)
        {
            _documents[key] = obj;
        }
    }
}