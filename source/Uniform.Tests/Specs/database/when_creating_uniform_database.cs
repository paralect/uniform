using System;
using Machine.Specifications;

namespace Uniform.Tests.Specs.database
{
    public class when_creating_uniform_database : _database_context
    {
        Because of = () =>
        {
            var db = UniformDatabase.Create(c => c
                .RegisterDocument<SampleDocument>()
            );

            db.GetCollection<SampleDocument>("sample_db", "sample_collection");

        };

        It should_be_aga = () =>
            false.ShouldBeFalse();
    }
}