using System;

namespace Uniform
{
    /// <summary>
    /// Simple abstraction for document database. Documents organized in collections. 
    /// Each collection can contains only one type of documents.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Gets collection with specifed name that contains documents of specified type (TDocument)
        /// Will be created, if not already exists.
        /// </summary>
        ICollection<TDocument> GetCollection<TDocument>(String name);

        /// <summary>
        /// Gets collection that contains documents of specified type (TDocument). Will be created, if not already exists.
        /// Name of collection will be taken from [Collection] attribute, that you can put on document class.
        /// If no [Collection] attribute found - type(TDocument).Name will be used for name.
        /// </summary>
        ICollection<TDocument> GetCollection<TDocument>();
    }
}