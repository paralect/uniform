using System;
using System.Collections.Generic;
using Uniform.Storage.Attributes;
using Uniform.Storage.Utils;

namespace Uniform.Storage.InMemory
{
    public class InMemoryDatabase : IDatabase
    {
        private readonly DatabaseMetadata _metadata;
        private readonly Dictionary<String, InMemoryCollection> _collections = new Dictionary<string, InMemoryCollection>();

        public InMemoryDatabase(DatabaseMetadata metadata)
        {
            _metadata = metadata;
        }

        public ICollection GetCollection(String name)
        {
            InMemoryCollection value;
            if (!_collections.TryGetValue(name, out value))
                _collections[name] = value = new InMemoryCollection();

            return value;
        }

        public ICollection<TDocument> GetCollection<TDocument>(string name)
        {
            return new ConcreteCollection<TDocument>(GetCollection(name));
        }

        public ICollection<TDocument> GetCollection<TDocument>()
        {
            return GetCollection<TDocument>(_metadata.GetCollectionName(typeof (TDocument)));
        }
    }
}