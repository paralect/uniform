using System;
using System.Collections.Generic;
using Uniform.Storage.Attributes;
using Uniform.Storage.Utils;

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

        public ICollection<TDocument> GetCollection<TDocument>()
        {
            var collectionAttribute = ReflectionHelper.GetSingleAttribute<CollectionAttribute>(typeof (TDocument));
            if (collectionAttribute == null)
                throw new Exception(String.Format("Collection attribute for document {0} was not specified", typeof(TDocument).FullName));

            return GetCollection<TDocument>(collectionAttribute.CollectionName);
        }
    }
}