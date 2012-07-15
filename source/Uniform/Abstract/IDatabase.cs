using System;

namespace Uniform
{
    /// <summary>
    /// Simple abstraction for document database. Documents organized in collections. 
    /// Each collection can contains only one type of documents.
    /// </summary>
    public interface IDatabase
    {
        void Initialize(UniformDatabase database);

        /// <summary>
        /// Gets collection with specifed name that contains documents of specified type (TDocument)
        /// Will be created, if not already exists.
        /// </summary>
        ICollection<TDocument> GetCollection<TDocument>(String name) where TDocument : new();
    }
}