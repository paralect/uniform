using System;

namespace Uniform.Storage
{
    public interface ICollection<TDocument> : ICollection
    {
        new TDocument GetById(String key);
        void Save(String key, TDocument obj);
        void Save(String key, Action<TDocument> creator);
        void Update(String key, Action<TDocument> updater);
    }

    public interface ICollection
    {
        Object GetById(String key);
        void Save(String key, Object obj);
        void Update(String key, Action<Object> updater);
    }
}