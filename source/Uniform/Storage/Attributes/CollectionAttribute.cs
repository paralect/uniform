using System;

namespace Uniform.Storage.Attributes
{
    public class CollectionAttribute
    {
        public string CollectionName { get; set; }

        public CollectionAttribute(String collectionName)
        {
            CollectionName = collectionName;
        }
    }
}