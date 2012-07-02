using System;
using System.Collections.Generic;
using System.Reflection;
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
                throw new Exception(String.Format("Collection attribute for document {0} was not specified", documentType.FullName));

            return collectionAttribute.CollectionName;
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