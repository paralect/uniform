using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Uniform.Mongodb
{
    public class MongodbBulkLoader
    {
        private readonly MongoDatabase _mongoDatabase;
        private readonly DatabaseMetadata _metadata;

        public MongodbBulkLoader(MongoDatabase mongoDatabase, DatabaseMetadata metadata)
        {
            _mongoDatabase = mongoDatabase;
            _metadata = metadata;
        }

        public void Load(BulkLoad load)
        {
            var tasks = new Task[load.Collections.Count];

            int i = 0;
            foreach (var pair in load.Collections)
            {
                var bulkCollection = pair.Value;
                var collectionName = pair.Key;
                var keys = bulkCollection.Documents.Keys;

                var bsonIdArray = new BsonArray(keys);

                var collection = _mongoDatabase.GetCollection(bulkCollection.CollectionType, collectionName);

                tasks[i] = Task.Factory.StartNew(() =>
                {
                    MongoCursor cursor = collection.FindAs(bulkCollection.CollectionType, Query.In("_id", bsonIdArray));

                    foreach (var doc in cursor)
                    {
                        var id = _metadata.GetDocumentId(doc);
                        bulkCollection.Documents[id] = doc;
                    }
                });
                i++;
            }
            Task.WaitAll(tasks);
        }
    }

    public class BulkLoad
    {
        public Dictionary<String, BulkLoadCollection> Collections = new Dictionary<string, BulkLoadCollection>();
    }

    public class BulkLoadCollection
    {
        public Type CollectionType;
        public ConcurrentDictionary<String, Object> Documents = new ConcurrentDictionary<string, object>();

        public BulkLoadCollection(Type collectionType)
        {
            CollectionType = collectionType;
        }
    }
}