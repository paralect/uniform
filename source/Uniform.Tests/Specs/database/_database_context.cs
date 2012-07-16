using System;

namespace Uniform.Tests.Specs.database
{
    public class _database_context
    {
         
    }

    [Document]
    public class SampleDocument
    {
        public String UserId { get; set; }
        public String About { get; set; }
    }
}