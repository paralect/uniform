using System;

namespace Uniform
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class DocumentAttribute : Attribute
    {
        public DocumentAttribute()
        {
        }
    }
}