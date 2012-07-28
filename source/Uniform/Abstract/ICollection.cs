using System;
using System.Collections.Generic;
using System.Linq;

namespace Uniform
{
    /// <summary>
    /// Simple abstraction that represents typed collection of documents.
    /// Collection always consists only of one type of documents.
    /// </summary>
    public interface ICollection<TDocument> : ICollection where TDocument : new()
    {
        /// <summary>
        /// Returns document by it's key. 
        /// If document doesn't exists - default(TDocument) will be returned.
        /// </summary>
        new TDocument GetById(String key);

        /// <summary>
        /// Returns documents by theirs key. 
        /// If some document doesn't exists - it will be skipped. So it is possible to receive 5 documents
        /// when you asked for 20.
        /// </summary>
        new IEnumerable<TDocument> GetById(IEnumerable<String> keys);

        /// <summary>
        /// Saves document to collection using specified key.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        void Save(String key, TDocument obj);

        /// <summary>
        /// Saves document to collection using specified key.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        void Save(TDocument obj);

        /// <summary>
        /// Saves document to collection using specified key. 
        /// 'Creator' function will be applied to automatically created document of type TDocument.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        void Save(String key, Action<TDocument> creator);

        /// <summary>
        /// Bulk save
        /// </summary>
        void Save(IEnumerable<TDocument> docs);

        /// <summary>
        /// Updates document with specified key.
        /// If document with such key doesn't exist, update will be discarded - i.e. no changes  to collection will be made. 
        /// See UpdateOrSave() method, if you need a kind of "upsert" behaviour.
        /// </summary>
        void Update(String key, Action<TDocument> updater);

        /// <summary>
        /// Updates document with specified key.
        /// If document with such key doesn't exists, new document will be created and 'updater' function will be applied to 
        /// this newly created document.
        /// </summary>
        void UpdateOrSave(String key, Action<TDocument> updater);
    }

    public interface ICollection
    {
        /// <summary>
        /// Returns document by it's key. 
        /// If document doesn't exists - default(TDocument) will be returned.
        /// </summary>
        Object GetById(String key);

        /// <summary>
        /// Returns documents by theirs key. 
        /// If some document doesn't exists - it will be skipped. So it is possible to receive 5 documents
        /// when you asked for 20.
        /// </summary>
        IEnumerable<Object> GetById(IEnumerable<String> keys);

        /// <summary>
        /// Saves document to collection using specified key.
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        void Save(String key, Object obj);

        /// <summary>
        /// Saves document to collection
        /// If document with such key already exists, it will be silently overwritten.
        /// </summary>
        void Save(Object obj);

        /// <summary>
        /// Bulk save
        /// </summary>
        void Save(IEnumerable<Object> docs);

        /// <summary>
        /// Deletes document with specified key.
        /// If document with such key doesn't exists - no changes to collection will be made.
        /// </summary>
        void Delete(String key);

        void DropAndPrepare();
    }
}