using System;
using System.Linq;
using System.Reflection;

namespace Uniform.Utils
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Returns attribute instance for specified type. Will return default type value if not found or not single.
        /// </summary>
        public static TAttribute GetSingleAttribute<TAttribute>(MemberInfo type)
        {
            var identities = type.GetCustomAttributes(typeof(TAttribute), false);

            if (identities.Length != 1)
                return default(TAttribute);

            if (!(identities[0] is TAttribute))
                return default(TAttribute);

            return (TAttribute)identities[0];
        }

        /// <summary>
        /// Returns attribute instance for specified type. Will return default type value if not found or not single.
        /// </summary>
        public static TAttribute[] GetAllAttributes<TAttribute>(MemberInfo type)
        {
            var identities = type.GetCustomAttributes(typeof(TAttribute), false);

            if (identities.Length == 0)
                return null;

            return identities.Cast<TAttribute>().ToArray();
        }
    }
}