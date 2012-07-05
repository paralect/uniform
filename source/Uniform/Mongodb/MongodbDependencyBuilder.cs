using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Uniform.Mongodb
{
    public class MongodbDependencyBuilder
    {
        private readonly DatabaseMetadata _metadata;

        public MongodbDependencyBuilder(DatabaseMetadata metadata)
        {
            _metadata = metadata;
        }

        public String BuildQueryString(List<PropertyInfo> infos)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var propertyInfo in infos)
            {
                builder.Append(propertyInfo.Name);
                builder.Append(".");
            }

            builder.Append("_id");
            return builder.ToString();
        }

        public String BuildUpdateString(List<PropertyInfo> infos)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < infos.Count; i++)
            {
                var propertyInfo = infos[i];
                builder.Append(propertyInfo.Name);

                if (i != infos.Count - 1)
                    builder.Append(".");
            }

            return builder.ToString();
        }

        public QueryComplete PathToQuery(List<PropertyInfo> infos, String key)
        {
            return Query.EQ(BuildQueryString(infos), key);
        }

        public UpdateBuilder PathToUpdate(List<PropertyInfo> infos, BsonValue value)
        {
            return Update.Set(BuildUpdateString(infos), value);
        }

    }
}