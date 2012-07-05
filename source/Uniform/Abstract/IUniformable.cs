using System;
using System.Collections.Generic;

namespace Uniform
{
    public interface IUniformable<T> : IEnumerable<T>
    {
        IUniformableProvider Provider { get; } 
    }
}