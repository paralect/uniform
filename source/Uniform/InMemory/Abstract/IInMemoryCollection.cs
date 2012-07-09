using System;
using System.Collections.Generic;

namespace Uniform.InMemory
{
    public interface IInMemoryCollection
    {
        Dictionary<String, Object> Documents { get; }
    }
}