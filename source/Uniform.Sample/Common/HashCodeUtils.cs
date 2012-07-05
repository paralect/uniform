using System;

namespace Uniform.Common
{
    public class HashCodeUtils
    {
        /// <summary>
        /// Compute the hash code for the given items
        /// </summary>
        public static Int32 Compute(params object[] values)
        {
            if (values == null)
                return 0;

            var hashCode = 27;
            foreach (object value in values)
            {
                if (value == null)
                    continue;

                hashCode = 13 * hashCode + value.GetHashCode();
            }

            return hashCode;
        }
    }
}