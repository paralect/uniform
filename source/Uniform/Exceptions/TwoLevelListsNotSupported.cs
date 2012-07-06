using System;

namespace Uniform.Exceptions
{
    public class TwoLevelListsNotSupported : Exception
    {
        public TwoLevelListsNotSupported(Type type1, Type type2) : base(String.Format(
            "Two level lists detected in dependency path between types '{0}' and '{1}'", 
            type1.FullName, type2.FullName))
        {
        }        
    }
}