using System;

namespace Uniform.Storage
{
    public interface IDatabase
    {
        ICollection GetCollection(String name);
        ICollection<TDocument> GetCollection<TDocument>(String name);
        ICollection<TDocument> GetCollection<TDocument>();
    }
}