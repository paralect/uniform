using System;
using Machine.Specifications;
using Uniform.Exceptions;
using Uniform.Temp.Metadata;

namespace Uniform.Tests.Specs.metadata.two_level_lists
{
    public class when_metadata_created_for_types_that_have_two_level_lists_in_dependency_path : _two_level_lists_context
    {
        Because of = () =>
        {
            circularException = Catch.Exception(() =>
            {
                Uniform.Temp.Metadata.DatabaseMetadata.Create(config => config
                    .AddDocumentType<User>()
                    .AddDocumentType<Student>()
                    .AddDocumentType<School>()
                    .AddDocumentType<District>()
                    .SetTwoLevelListsSupport(false)
                );
            });
        };

        It should_throw_circular_exception = () =>
            circularException.ShouldBeOfType<TwoLevelListsNotSupported>();

        private static Exception circularException;
    }
}