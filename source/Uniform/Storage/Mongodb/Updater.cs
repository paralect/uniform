using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Uniform.Storage.Mongodb
{
    public class Updater
    {
        private readonly DatabaseMetadata _metadata;

        public Updater(DatabaseMetadata metadata)
        {
            _metadata = metadata;
        }

        public void Update(Object obj, List<PropertyInfo> infos, Object value)
        {
            var id = _metadata.GetDocumentId(value);
            if (id == null)
                throw new Exception("Object don't have valid id");

            var prop = Find(obj, infos, id, 0).ToList();

            if (prop.Count == 0)
                return;

            foreach (var foundEntry in prop)
            {
                if (foundEntry.Index == -1)
                {
                    foundEntry.PropertyInfo.SetValue(foundEntry.Document, value, new object[0]);
                }
                else
                {
                    var list = (IList) foundEntry.PropertyInfo.GetValue(foundEntry.Document, new object[0]);
                    list[foundEntry.Index] = value;
                }

            }
        }

        public IEnumerable<FoundEntry> Find(Object obj, List<PropertyInfo> infos, Object key, Int32 current)
        {
            var currentInfo = infos[current];

            if (IsList(currentInfo.PropertyType))
            {
                var list = (IList) currentInfo.GetValue(obj, new object[0]);

                for (int index = 0; index < list.Count; index++)
                {
                    var inner = list[index];
                    var parentInfo = new ParentInfo(obj, currentInfo, index);

                    var result = FindObject(parentInfo, inner, infos, key, current + 1);

                    foreach (var foundEntry in result)
                        yield return foundEntry;
                }
            }
            else
            {
                var value = currentInfo.GetValue(obj, new object[0]);
                if (value == null)
                    yield break;

                var parentInfo = new ParentInfo(obj, currentInfo, -1);
                var result = FindObject(parentInfo, value, infos, key, current + 1);

                foreach (var foundEntry in result)
                    yield return foundEntry;
            }
        }

        private IEnumerable<FoundEntry> FindObject(ParentInfo parentInfo, Object obj, List<PropertyInfo> infos, Object key, Int32 current)
        {
            if (current >= infos.Count)
            {
                var id = _metadata.GetDocumentId(obj);

                if (!id.Equals(key))
                    yield break;

                yield return new FoundEntry(parentInfo.ParentObject, parentInfo.ParentPropertyInfo, parentInfo.Index);
                yield break;                
            }

            foreach (var entry in Find(obj, infos, key, current))
                yield return entry;
        }

        /// <summary>
        /// Only List(T) supported for now
        /// </summary>
        private Boolean IsList(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return true;

            return false;
        }
         
    }

    public class FoundEntry
    {
        public Int32 Index { get; set; }
        public Object Document { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public FoundEntry(object document, PropertyInfo propertyInfo, Int32 index)
        {
            Document = document;
            PropertyInfo = propertyInfo;
            Index = index;
        }
    }

    public class ParentInfo
    {
        public Object ParentObject { get; set; }
        public PropertyInfo ParentPropertyInfo { get; set; }
        public Int32 Index { get; set; }

        public ParentInfo(object parentObject, PropertyInfo parentPropertyInfo, int index)
        {
            ParentObject = parentObject;
            ParentPropertyInfo = parentPropertyInfo;
            Index = index;
        }
    }
}