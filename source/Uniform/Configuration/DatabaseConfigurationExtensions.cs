using System;
using System.Collections.Generic;
using System.Reflection;
using Uniform.Utils;
using System.Linq;

namespace Uniform.Configuration
{
    public static class DatabaseConfigurationExtensions
    {
        /// <summary>
        /// Registers collection for specified document type and with specified name 
        /// </summary>
        public static DatabaseConfiguration RegisterDocument<TDocument>(this DatabaseConfiguration configuration, String databaseName, String collectionName)
        {
            configuration.RegisterDocument(databaseName, collectionName, typeof (TDocument));
            return configuration;
        }

        /// <summary>
        /// Registers collection for specified document type. 
        /// Name of collection will be discovered by looking to [Collection] attribute of TDocument type.
        /// </summary>
        public static DatabaseConfiguration RegisterDocument<TDocument>(this DatabaseConfiguration configuration)
        {
            var collectionAttribute = ReflectionHelper.GetSingleAttribute<DocumentAttribute>(typeof (TDocument));
            var collectionName = collectionAttribute.CollectionName;
            var databaseName = collectionAttribute.DatabaseName;
            configuration.RegisterDocument(databaseName, collectionName, typeof(TDocument));
            return configuration;
        }

        /// <summary>
        /// Registers many collections by scanning specified assembly for types that decorated with [Collection] attribute. 
        /// You can optionaly specify fullNamePrefix parameter to register types only with this prefix.
        /// Name of each collection will be discovered by looking to [Collection] attribute of TDocument type.
        /// </summary>
        public static DatabaseConfiguration RegisterCollections(this DatabaseConfiguration configuration, Assembly assembly, String fullNamePrefix = null)
        {
            var result = GetTypesWithAttribute<DocumentAttribute>(new[] { assembly });

            if (fullNamePrefix != null)
                result = result.Where(t => t.Item1.FullName != null && t.Item1.FullName.StartsWith(fullNamePrefix));

            foreach (var tuple in result)
                configuration.RegisterDocument(tuple.Item2.DatabaseName, tuple.Item2.CollectionName, tuple.Item1);
            
            return configuration;
        }

        private static IEnumerable<Tuple<Type, TAttribute>> GetTypesWithAttribute<TAttribute>(params Assembly[] assemblies)
        {
            var typesWithAttribute =
                from a in assemblies
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof (TAttribute), true)
                where attributes != null && attributes.Length > 0
                select new Tuple<Type, TAttribute>(t, (TAttribute) attributes[0]);

            return typesWithAttribute;
        }        
    }
}