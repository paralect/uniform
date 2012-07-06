using System;
using System.Linq;

namespace Uniform
{
    public interface ICollection<TDocument>
    {
        new TDocument GetById(String key);
        void Save(String key, TDocument obj);
        void Save(String key, Action<TDocument> creator);
        void Update(String key, Action<TDocument> updater);
        void Delete(String key);
    }
}