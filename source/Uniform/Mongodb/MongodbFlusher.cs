using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Uniform.InMemory;

namespace Uniform.Mongodb
{
    public class MongodbFlusher
    {
        /// <summary>
        /// MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// Name of MongoDB database 
        /// </summary>
        private readonly string _databaseName;

        /// <summary>
        /// In-memory database, from which we'll get data from
        /// </summary>
        private readonly InMemoryDatabase _inMemoryDatabase;

        /// <summary>
        /// MongoDB Server
        /// </summary>
        private MongoServer Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Get database
        /// </summary>
        public MongoDatabase Database
        {
            get { return _server.GetDatabase(_databaseName); }
        }

        public MongodbFlusher(InMemoryDatabase inMemoryDatabase, String connectionString)
        {
            _inMemoryDatabase = inMemoryDatabase;
            _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            _server = MongoServer.Create(connectionString);
            Database.Drop();
        }

        public void Flush()
        {
            long tobson = 0;

            int index = 0;
            Task[] tasks = new Task[_inMemoryDatabase.Collections.Keys.Count];

            foreach (var pair in _inMemoryDatabase.Collections)
            {
                var mongoSettings = Database.CreateCollectionSettings(typeof (BsonDocument), pair.Key);
                mongoSettings.AssignIdOnInsert = false;
                //mongoSettings.SafeMode = SafeMode.False;

                var mongoCollection = Database.GetCollection(mongoSettings);
                var inMemoryCollection = (IInMemoryCollection) pair.Value;


/*
                foreach (var docPair in inMemoryCollection.Documents)
                {
                    var doc = BsonDocumentWrapper.Create(docPair.Value.GetType(), docPair.Value);
                    mongoCollection.Insert(doc);
                }
*/
                var stopwatch = Stopwatch.StartNew();
                var docs = BsonDocumentWrapper.CreateMultiple(inMemoryCollection.Documents.Values);
                stopwatch.Stop();
                Console.WriteLine("Collection {0} serialized to bson in {1:n0} ms", pair.Key, stopwatch.ElapsedMilliseconds);
                tobson += stopwatch.ElapsedMilliseconds;

                stopwatch.Start();

                tasks[index] = Task.Factory.StartNew(() =>
                {
                    var mongoInsertOptions = new MongoInsertOptions();
                    mongoInsertOptions.CheckElementNames = false;
                    mongoInsertOptions.SafeMode = SafeMode.True;
                    mongoCollection.InsertBatch(docs);
                }, TaskCreationOptions.LongRunning);

                
                stopwatch.Stop();
                Console.WriteLine("Collection {0} inserted to MongoDB in {1:n0} ms", pair.Key, stopwatch.ElapsedMilliseconds);

                index++;
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Total time for serialization: {0:n0} ms", tobson);
        }
    }
}