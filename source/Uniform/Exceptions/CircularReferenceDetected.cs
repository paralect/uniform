using System;

namespace Uniform.Exceptions
{
    public class CircularDependencyNotSupportedException : Exception
    {
        public CircularDependencyNotSupportedException(Type type1, Type type2) 
            : base(String.Format("Circular reference detected between types '{0}' and '{1}'", type1.FullName, type2.FullName))
        {
        }
    }
}