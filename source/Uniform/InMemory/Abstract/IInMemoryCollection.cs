using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Uniform.InMemory
{
    public interface IInMemoryCollection
    {
        ConcurrentDictionary<String, Object> Documents { get; }
    }
}