using System;

namespace Uniform.Storage.Attributes
{
    public class CollectionAttribute : Attribute
    {
        public string CollectionName { get; set; }

        public CollectionAttribute(String collectionName)
        {
            CollectionName = collectionName;
        }
    }
}