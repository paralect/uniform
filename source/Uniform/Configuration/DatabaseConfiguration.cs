using System;
using System.Collections.Generic;

namespace Uniform
{
    public class DatabaseConfiguration
    {
        public Dictionary<Type, DocumentConfiguration> DocumentConfigurations { get; set; }

        public Dictionary<String, IDocumentDatabase> Databases { get; set; } 

        public DatabaseConfiguration()
        {
            DocumentConfigurations = new Dictionary<Type, DocumentConfiguration>();
            Databases = new Dictionary<string, IDocumentDatabase>();
        }

        /// <summary>
        /// Registers single document
        /// </summary>
        public void RegisterDocument(Type documentType)
        {
            var documentConfiguration = new DocumentConfiguration(documentType);
            DocumentConfigurations[documentType] = documentConfiguration;
        }
    }
}