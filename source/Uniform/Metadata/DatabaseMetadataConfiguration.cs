using System;
using System.Collections.Generic;

namespace Uniform
{
    public class DatabaseMetadataConfiguration
    {
        public Boolean TwoLevelListSupported { get; set; }
        public List<Type> DocumentTypes { get; set; }

        public DatabaseMetadataConfiguration()
        {
            DocumentTypes = new List<Type>();
            TwoLevelListSupported = false;
        }
    }
}