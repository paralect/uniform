using System;

namespace Uniform
{
    public interface IDatabase
    {
        ICollection<TDocument> GetCollection<TDocument>(String name);
        ICollection<TDocument> GetCollection<TDocument>();
    }
}