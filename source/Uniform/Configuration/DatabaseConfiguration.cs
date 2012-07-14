using System;
using System.Collections.Generic;

namespace Uniform.Configuration
{
    public class DatabaseConfiguration
    {
        public HashSet<DocumentConfiguration> Collections { get; set; }

        public DatabaseConfiguration()
        {
            Collections = new HashSet<DocumentConfiguration>();
        }

        public void RegisterDocument(string databaseName, string collectionName, Type documentType)
        {
            Collections.Add(new DocumentConfiguration(databaseName, collectionName, documentType));
        }
    }
}