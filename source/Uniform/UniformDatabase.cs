﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using NLog;
using Uniform.InMemory;

namespace Uniform
{
    public class UniformDatabase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Boolean _inMemory = false;
        private Dictionary<String, IDocumentDatabase> _databases = new Dictionary<string, IDocumentDatabase>();

        private readonly DatabaseMetadata _metadata;

        public DatabaseMetadata Metadata
        {
            get { return _metadata; }
        }

        private UniformDatabase(DatabaseConfiguration configuration)
        {
            _metadata = new DatabaseMetadata(configuration);

            // Initialize all databases
            foreach (var database in _metadata.Databases)
                database.Value.Initialize(this);
        }

        /// <summary>
        /// Factory method, that creates DatabaseMetadata
        /// </summary>
        public static UniformDatabase Create(Action<DatabaseConfiguration> configurator)
        {
            var configuration = new DatabaseConfiguration();
            configurator(configuration);
            var database = new UniformDatabase(configuration);
            return database;
        }

        public IDocumentCollection<TDocument> GetCollection<TDocument>(String databaseName, String collectionName) where TDocument : new()
        {
            return _metadata.Databases[databaseName].GetCollection<TDocument>(collectionName);
        }

        public void EnterInMemoryMode()
        {
            _inMemory = true;
            _databases = _metadata.Databases;

            var newDatabases = new Dictionary<String, IDocumentDatabase>();

            foreach (var database in _metadata.Databases)
            {
                var inmemory = new InMemoryDatabase();
                inmemory.Initialize(this);
                newDatabases[database.Key] = inmemory;
            }
                
            _metadata.Databases = newDatabases;
        }

        public void LeaveInMemoryMode(bool flush = false, bool dropBeforeFlush = true)
        {
            if (!_inMemory)
                return;

            _inMemory = false;
            var inmemoryDB = _metadata.Databases;
            _metadata.Databases = _databases;

            if (flush)
                Flush(inmemoryDB, _databases, dropBeforeFlush);
        }

        private void Flush(Dictionary<string, IDocumentDatabase> @from, Dictionary<string, IDocumentDatabase> to, bool dropBeforeFlush)
        {
            var flush = new ActionBlock<FlushTo>(s =>
            {
                try
                {
                    if (dropBeforeFlush)
                        s.To.DropAndPrepare();

                    if (s.Data.Count > 0)
                        s.To.Save(s.Data);
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex);
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 10 });

            foreach (var inMemoryDatabasePair in from)
            {
                var inMemoryDatabase = (InMemoryDatabase) inMemoryDatabasePair.Value;
                var destinationDatabase = to[inMemoryDatabasePair.Key];

                if (dropBeforeFlush)
                    destinationDatabase.DropAllCollections();

                foreach (var inMemoryCollectionPair in inMemoryDatabase.Collections)
                {
                    var inMemoryCollection = (IInMemoryCollection)inMemoryCollectionPair.Value;
                    var documentType = inMemoryCollectionPair.Key.DocumentType;
                    var collectionName = inMemoryCollectionPair.Key.CollectionName;

                    var destinationCollection = destinationDatabase.GetCollection(documentType, collectionName);

                    if (dropBeforeFlush)
                        destinationCollection.DropAndPrepare();

                    flush.Post(new FlushTo(destinationCollection, inMemoryCollection.Documents.Values));
                }
            }

            flush.Complete();
            flush.Completion.Wait();
        }
    }

    public class FlushTo
    {
        public IDocumentCollection To { get; set; }
        public ICollection<Object> Data { get; set; }

        public FlushTo(IDocumentCollection to, ICollection<object> data)
        {
            To = to;
            Data = data;
        }
    }
}