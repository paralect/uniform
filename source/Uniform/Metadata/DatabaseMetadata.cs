using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.Exceptions;
using Uniform.Utils;

namespace Uniform
{
    /// <summary>
    /// Database metadata, contains all document types and provides some
    /// metadata related services.
    /// </summary>
    public class DatabaseMetadata
    {
        /// <summary>
        /// Configuration of DatabaseMetadata, used to create this type
        /// </summary>
        private readonly DatabaseMetadataConfiguration _configuration;

        /// <summary>
        /// All registered document types. 
        /// </summary>
        private readonly Dictionary<Type, DocumentInfo> _documentTypes = new Dictionary<Type, DocumentInfo>();

        public IEnumerable<Type> DocumentTypes
        {
            get { return _documentTypes.Keys; }
        }

        /// <summary>
        /// Creates DatabaseMetadata with specified configuration
        /// </summary>
        /// <param name="configuration"></param>
        public DatabaseMetadata(DatabaseMetadataConfiguration configuration)
        {
            _configuration = configuration;
            foreach (var documentType in configuration.DocumentTypes)
                _documentTypes[documentType] = new DocumentInfo(documentType);
        }

        /// <summary>
        /// Factory method, that creates DatabaseMetadata
        /// </summary>
        public static DatabaseMetadata Create(Action<DatabaseMetadataConfiguration> configurator)
        {
            var configuration = new DatabaseMetadataConfiguration();
            configurator(configuration);
            var metadata = new DatabaseMetadata(configuration);
            metadata.Analyze();
            return metadata;
        }

        private void Analyze()
        {
            foreach (var documentInfo in _documentTypes.Values)
                AnalyzeType(documentInfo.DocumentType, new List<PropertyInfo>(),  documentInfo.DocumentType);
        }

        private void AnalyzeType(Type originalType, List<PropertyInfo> path, Type type)
        {
            var infos = type.GetProperties();

            foreach (var propertyInfo in infos)
            {
                Type documentType;
                Boolean isList = IsList(propertyInfo.PropertyType, out documentType);

                if (!isList)
                    documentType = propertyInfo.PropertyType;

                if (!IsDocumentType(documentType))
                    continue;

                if (originalType == documentType)
                    throw new CircularDependencyNotSupportedException(documentType, type);

                if (!_configuration.TwoLevelListSupported && isList && PathContainsLists(path))
                    throw new TwoLevelListsNotSupported(documentType, type);

                // Copy path to new list
                var newPath = new List<PropertyInfo>(path);

                // add current propertyInfo to path
                newPath.Add(propertyInfo);

                var dep = new DependentDocumentMetadata(originalType, new List<PropertyInfo>(newPath));

                var list = GetDependents(documentType);
                list.Add(dep);

                if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType != typeof(String))
                    AnalyzeType(originalType, newPath, documentType);
            }
        }

        private Boolean PathContainsLists(IEnumerable<PropertyInfo> path)
        {
            foreach (var info in path)
            {
                Type itemType;
                if (IsList(info.PropertyType, out itemType))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Only List(T) supported for now
        /// </summary>
        private Boolean IsList(Type type, out Type itemType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                itemType = type.GetGenericArguments()[0];
                return true;
            }

            itemType = null;
            return false;
        }

        /// <summary>
        /// Get list of all types, that depends on specified documentType
        /// </summary>
        public List<DependentDocumentMetadata> GetDependents(Type documentType)
        {
            DocumentInfo value;
            if (!_documentTypes.TryGetValue(documentType, out value))
                throw new Exception(String.Format("Document type '{0}' wasn't registered in metdata", documentType.FullName));

            return value.Dependents;
        }

        /// <summary>
        /// Returns true, if type was registered as document type
        /// </summary>
        public bool IsDocumentType(Type type)
        {
            return _documentTypes.ContainsKey(type);
        }

        /// <summary>
        /// Returns collection name for specified document type
        /// </summary>
        public String GetCollectionName(Type documentType)
        {
            DocumentInfo info;
            if (!_documentTypes.TryGetValue(documentType, out info))
                throw new Exception(String.Format("Document type '{0}' wasn't registered in metdata", documentType.FullName));

            if (info.CollectionName == null)
            {
                var collectionAttribute = ReflectionHelper.GetSingleAttribute<CollectionAttribute>(documentType);
                info.CollectionName = collectionAttribute == null ? documentType.Name : collectionAttribute.CollectionName;
            }

            return info.CollectionName;
        }

        #region ID properties services
        /// <summary>
        /// Cache for ID properties. (DocumentType -> Id PropertyInfo)
        /// </summary>
        private readonly Dictionary<Type, PropertyInfo> _idPropertiesCache = new Dictionary<Type, PropertyInfo>();

        /// <summary>
        /// Returns document id value. 
        /// </summary>
        public String GetDocumentId(Object document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            var info = GetDocumentIdPropertyInfo(document.GetType());
            return (String) info.GetValue(document, new object[0]);
        }

        /// <summary>
        /// Sets id property to specified value. 
        /// </summary>
        public void SetDocumentId(Object obj, String value)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var info = GetDocumentIdPropertyInfo(obj.GetType());
            info.SetValue(obj, value, new object[0]);
        }

        public PropertyInfo GetDocumentIdPropertyInfo(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            PropertyInfo info;
            if (!_idPropertiesCache.TryGetValue(type, out info))
            {
                PropertyInfo[] propertyInfos = type.GetProperties()
                    .Where(x => Attribute.IsDefined(x, typeof(BsonIdAttribute), false))
                    .ToArray();

                if (propertyInfos.Length <= 0)
                    throw new Exception(String.Format(
                        "Document of type '{0}' does not have id property, marked with [BsonId] attribute. Please mark it :)'", type.FullName));

                _idPropertiesCache[type] = info = propertyInfos[0];
            }

            return info;
        }

        #endregion
    }

    public class DependentDocumentMetadata
    {
        public Type DependentDocumentType { get; set; }
        public List<PropertyInfo> SourceDocumentPath { get; set; }

        public DependentDocumentMetadata(Type dependentDocumentType, List<PropertyInfo> sourceDocumentPath)
        {
            DependentDocumentType = dependentDocumentType;
            SourceDocumentPath = sourceDocumentPath;
        }

        public DependentDocumentMetadata()
        {
            SourceDocumentPath = new List<PropertyInfo>();
        }
    }
}