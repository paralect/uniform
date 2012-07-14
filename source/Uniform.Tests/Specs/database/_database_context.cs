using System;

namespace Uniform.Tests.Specs.database
{
    public class _database_context
    {
         
    }

    [Document("sample_db", "sample_collection")]
    [Document("sample_db1", "sample_collection")]
    public class SampleDocument
    {
        public String UserId { get; set; }
        public String About { get; set; }
    }
}