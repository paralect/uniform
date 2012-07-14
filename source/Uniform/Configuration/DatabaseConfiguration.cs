using System;
using System.Collections.Generic;

namespace Uniform
{
    public class DatabaseConfiguration
    {
        public HashSet<DocumentConfiguration> DocumentConfigurations { get; set; }
        public Dictionary<String, IDatabase> Databases { get; set; } 

        public DatabaseConfiguration()
        {
            DocumentConfigurations = new HashSet<DocumentConfiguration>();
            Databases = new Dictionary<string, IDatabase>();
        }

        /// <summary>
        /// Registers single document
        /// </summary>
        public void RegisterDocument(string databaseName, string collectionName, Type documentType)
        {
            if (databaseName == null) throw new ArgumentNullException("databaseName");
            if (collectionName == null) throw new ArgumentNullException("collectionName");

            var added = DocumentConfigurations.Add(new DocumentConfiguration(databaseName, collectionName, documentType));

            if (added == false)
                throw new Exception(String.Format("Duplicate registration for document of type {0}" +
                    " in '{1}' database and '{2}' collection.", documentType, databaseName, collectionName));
        }
    }
}