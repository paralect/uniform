using System;
using System.Collections.Generic;

namespace Uniform.Storage.InMemory
{
    public class InMemoryDatabase : IDatabase
    {
        private readonly Dictionary<String, InMemoryCollection> _collections = new Dictionary<string, InMemoryCollection>();

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
    }
}