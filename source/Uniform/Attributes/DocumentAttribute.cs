using System;

namespace Uniform
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class DocumentAttribute : Attribute
    {
        /// <summary>
        /// Name of database that contains collections of this document
        /// </summary>
        public String DatabaseName { get; set; }

        /// <summary>
        /// Name of collection that contains this document
        /// </summary>
        public String CollectionName { get; set; }
        
        /// <summary>
        /// Creates DocumentAttribute
        /// </summary>
        public DocumentAttribute(String databaseName, String collectionName)
        {
            CollectionName = collectionName;
            DatabaseName = databaseName;
        }
    }
}