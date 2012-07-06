using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.Attributes;
using Uniform.Utils;

namespace Uniform
{
    /// <summary>
    /// Database metadata, contains all document types and provides some
    /// metadata related services.
    /// </summary>
    public class DatabaseMetadata
    {
        private readonly List<Type> _documentTypes = new List<Type>();
        private readonly Dictionary<Type, List<DependentDocumentMetadata>> _map = new Dictionary<Type, List<DependentDocumentMetadata>>();

        public DatabaseMetadata(List<Type> documentTypes)
        {
            _documentTypes = documentTypes;
        }

        public void Analyze()
        {
            foreach (var type in _documentTypes)
            {
                AnalyzeType(type, new List<PropertyInfo>(),  type);
            }
        }

        private void AnalyzeType(Type originalType, List<PropertyInfo> path, Type type)
        {
            var infos = type.GetProperties();

            foreach (var propertyInfo in infos)
            {
                var newPath = new List<PropertyInfo>(path);
                newPath.Add(propertyInfo);

                if (IsDocumentType(propertyInfo.PropertyType))
                {
                    var dep = new DependentDocumentMetadata();
                    dep.DependentDocumentType = originalType;
                    dep.SourceDocumentPath = new List<PropertyInfo>(newPath);

                    var list = GetDependents(propertyInfo.PropertyType);
                    list.Add(dep);
                }

                if (!propertyInfo.PropertyType.IsPrimitive && propertyInfo.PropertyType != typeof(String))
                    AnalyzeType(originalType, newPath, propertyInfo.PropertyType);
            }
        }

        public List<DependentDocumentMetadata> GetDependents(Type type)
        {
            List<DependentDocumentMetadata> value;
            if (!_map.TryGetValue(type, out value))
                _map[type] = value = new List<DependentDocumentMetadata>();

            return value;
        }

        public bool IsDocumentType(Type type)
        {
            return _documentTypes.Contains(type);
        }

        private readonly Dictionary<Type, String> _cachedCollectionNames = new Dictionary<Type, string>();
        public String GetCollectionName(Type documentType)
        {
            string name;
            if (!_cachedCollectionNames.TryGetValue(documentType, out name))
            {
                var collectionAttribute = ReflectionHelper.GetSingleAttribute<CollectionAttribute>(documentType);
                name = collectionAttribute == null ? documentType.ToString() : collectionAttribute.CollectionName;
                _cachedCollectionNames[documentType] = name;
            }

            return name;
        }

        #region ID properties services
        /// <summary>
        /// Cache for ID properties. (DocumentType -> Id PropertyInfo)
        /// </summary>
        private readonly Dictionary<Type, PropertyInfo> _idPropertiesCache = new Dictionary<Type, PropertyInfo>();

        /// <summary>
        /// Returns document id value. 
        /// </summary>
        public Object GetDocumentId(Object document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            var info = GetDocumentIdPropertyInfo(document.GetType());
            return info.GetValue(document, new object[0]);
        }

        /// <summary>
        /// Sets id property to specified value. 
        /// </summary>
        public void SetDocumentId(Object obj, Object value)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var info = GetDocumentIdPropertyInfo(obj.GetType());
            info.SetValue(obj, value, new object[0]);
        }

        private PropertyInfo GetDocumentIdPropertyInfo(Type type)
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

        public DependentDocumentMetadata()
        {
            SourceDocumentPath = new List<PropertyInfo>();
        }
    }


}