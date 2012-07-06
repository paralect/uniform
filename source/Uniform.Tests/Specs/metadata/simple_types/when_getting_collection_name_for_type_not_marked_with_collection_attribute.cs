using System;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.simple_types
{
    public class when_getting_collection_name_for_type_not_marked_with_collection_attribute : _simple_types_context
    {
        Because of = () =>
            collectionName = metadata.GetCollectionName(typeof (User));

        It should_has_correct_name = () =>
            collectionName.ShouldEqual(typeof(User).Name);

        private static String collectionName;
    }
}