using System;
using System.Collections.Generic;
using System.Reflection;
using Uniform.Utils;
using System.Linq;

namespace Uniform
{
    public static class DatabaseConfigurationExtensions
    {
        /// <summary>
        /// Registers single document that belong to specified database and collection
        /// </summary>
        public static DatabaseConfiguration RegisterDocument<TDocument>(this DatabaseConfiguration configuration)
        {
            configuration.RegisterDocument(typeof (TDocument));
            return configuration;
        }

        /// <summary>
        /// Registers documents by scanning specified assembly for types that decorated with [Document] attribute. 
        /// You can optionaly specify fullNamePrefix parameter to register types only with this prefix.
        /// Name of database and collection for particular document will be discovered by looking to [Document] attribute 
        /// of TDocument type.
        /// </summary>
        public static DatabaseConfiguration RegisterDocuments(this DatabaseConfiguration configuration, Assembly assembly, String fullNamePrefix = null)
        {
            var result = GetTypesWithAttribute<DocumentAttribute>(new[] { assembly });

            if (fullNamePrefix != null)
                result = result.Where(t => t.Item1.FullName != null && t.Item1.FullName.StartsWith(fullNamePrefix));

            foreach (var tuple in result)
                foreach (var documentAttribute in tuple.Item2)
                    configuration.RegisterDocument(tuple.Item1);
            
            return configuration;
        }

        public static DatabaseConfiguration RegisterDatabase(this DatabaseConfiguration configuration, String databaseName, IDocumentDatabase database)
        {
            if (databaseName == null) throw new ArgumentNullException("databaseName");
            if (database == null) throw new ArgumentNullException("database");

            configuration.Databases[databaseName] = database;
            return configuration;
        }

        /// <summary>
        /// Helper method to find types that decorated with specified attribute
        /// Returns (type, attribute[]) tuples.
        /// </summary>
        private static IEnumerable<Tuple<Type, TAttribute[]>> GetTypesWithAttribute<TAttribute>(params Assembly[] assemblies)
        {
            var typesWithAttribute =
                from a in assemblies
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof (TAttribute), true)
                where attributes != null && attributes.Length > 0
                select new Tuple<Type, TAttribute[]>(t, attributes.Cast<TAttribute>().ToArray());

            return typesWithAttribute;
        }        
    }
}