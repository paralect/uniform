using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Storage
{
    public class DocumentHelper
    {
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
}