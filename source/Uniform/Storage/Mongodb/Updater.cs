using System;
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

        public void Update(Object obj, List<PropertyInfo> infos, Object key, Object value)
        {
            var prop = Find(obj, infos, key, 0).ToList();

            if (prop.Count == 0)
                return;

            foreach (var foundEntry in prop)
                foundEntry.PropertyInfo.SetValue(foundEntry.Document, value, new object[0]);
        }

        public IEnumerable<FoundEntry> Find(Object obj, List<PropertyInfo> infos, Object key, Int32 current)
        {
            var result = FindObject(obj, infos, key, current);

            foreach (var foundEntry in result)
                yield return foundEntry;
        }

        private IEnumerable<FoundEntry> FindObject(Object obj, List<PropertyInfo> infos, Object key, Int32 current)
        {
            var info = infos[current];

            if (current == infos.Count - 1)
            {
                // check that key is valid
                var value = info.GetValue(obj, new object[0]);
                if (value == null)
                    yield break;

                var id = _metadata.GetDocumentId(value);

                if (id != key)
                    yield break;

                yield return new FoundEntry(obj, info);
                yield break;
            }


            var nbj = info.GetValue(obj, new object[0]);

            if (nbj == null)
                yield break;

            foreach (var entry in Find(nbj, infos, key, current + 1))
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
        public Object Document { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public FoundEntry(object document, PropertyInfo propertyInfo)
        {
            Document = document;
            PropertyInfo = propertyInfo;
        }
    }
}