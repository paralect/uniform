using System;

namespace Uniform
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