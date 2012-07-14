using System;

namespace Uniform.Configuration
{
    public class DocumentConfiguration
    {
        public String DatabaseName { get; set; }
        public String CollectionName { get; set; }
        public Type DocumentType { get; set; }

        public DocumentConfiguration(String databaseName, String collectionName, Type documentType)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
            DocumentType = documentType;
        }
    }
}