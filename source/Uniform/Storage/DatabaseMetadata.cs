using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.Storage.Attributes;
using Uniform.Storage.Utils;

namespace Uniform.Storage
{
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
                AnalyzeType(type);
            }
        }

        private void AnalyzeType(Type type)
        {
            var infos = type.GetProperties();

            foreach (var propertyInfo in infos)
            {
                if (!IsDocumentType(propertyInfo.PropertyType))
                    continue;

                var dep = new DependentDocumentMetadata();
                dep.DependentDocumentType = type;
                dep.SourceDocumentPath.Add(propertyInfo);

                var list = GetDependents(propertyInfo.PropertyType);
                list.Add(dep);
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

        public String GetCollectionName(Type documentType)
        {
            var collectionAttribute = ReflectionHelper.GetSingleAttribute<CollectionAttribute>(documentType);
            if (collectionAttribute == null)
                return documentType.ToString();
                //throw new Exception(String.Format("Collection attribute for document {0} was not specified", documentType.FullName));

            return collectionAttribute.CollectionName;
        }

        private readonly Dictionary<Type, PropertyInfo> _cache = new Dictionary<Type, PropertyInfo>();

        public Object GetDocumentId(Object obj)
        {
            var info = GetDocumentIdPropertyInfo(obj.GetType());
            return info.GetValue(obj, new object[0]);
        }

        public void SetDocumentId(Object obj, Object value)
        {
            var info = GetDocumentIdPropertyInfo(obj.GetType());
            info.SetValue(obj, value, new object[0]);
        }

        private PropertyInfo GetDocumentIdPropertyInfo(Type type)
        {
            PropertyInfo info;
            if (!_cache.TryGetValue(type, out info))
            {
                PropertyInfo[] propertyInfos = type.GetProperties()
                    .Where(x => Attribute.IsDefined(x, typeof(BsonIdAttribute), false))
                    .ToArray();

                if (propertyInfos.Length <= 0)
                    throw new Exception(String.Format(
                        "Document of type '{0}' does not have id property, marked with [BsonId] attribute. Please mark it :)'", type.FullName));

                _cache[type] = info = propertyInfos[0];
            }

            return info;
        }

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