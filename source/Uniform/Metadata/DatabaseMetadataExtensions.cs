using System;

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
    }
}