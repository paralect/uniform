using System;

namespace Uniform
{
    public interface IDatabase
    {
        ICollection GetCollection(String name);
        ICollection<TDocument> GetCollection<TDocument>(String name);
        ICollection<TDocument> GetCollection<TDocument>();
    }
}