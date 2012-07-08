using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Uniform
{
    public static class DatabaseMetadataExtensions
    {
        public static DatabaseMetadataConfiguration AddDocumentType<TDocument>(this DatabaseMetadataConfiguration configuration)
        {
            configuration.DocumentTypes.Add(typeof (TDocument));
            return configuration;
        }

        public static DatabaseMetadataConfiguration AddDocumentType(this DatabaseMetadataConfiguration configuration, Type documentType)
        {
            configuration.DocumentTypes.Add(documentType);
            return configuration;
        }

        public static DatabaseMetadataConfiguration AddDocumentTypes(this DatabaseMetadataConfiguration configuration, Assembly assembly, String fullNamePrefix = null)
        {
            var result = GetTypesWithAttribute<CollectionAttribute>(new[] { assembly });

            if (fullNamePrefix != null)
                result = result
                    .Where(t => t.FullName != null && t.FullName.StartsWith(fullNamePrefix));

            configuration.DocumentTypes.AddRange(result);
            return configuration;
        }

        public static DatabaseMetadataConfiguration SetTwoLevelListsSupport(this DatabaseMetadataConfiguration configuration, Boolean supported)
        {
            configuration.TwoLevelListSupported = supported;
            return configuration;
        }


        private static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(params Assembly[] assemblies)
        {
            var typesWithAttribute =
                from a in assemblies
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof (TAttribute), true)
                where attributes != null && attributes.Length > 0 
                select t;

            return typesWithAttribute;
        }
    }
}