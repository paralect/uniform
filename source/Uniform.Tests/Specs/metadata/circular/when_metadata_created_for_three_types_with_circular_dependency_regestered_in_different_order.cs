using System;
using Machine.Specifications;
using Uniform.Exceptions;
using Uniform.Temp.Metadata;

namespace Uniform.Tests.Specs.metadata.circular
{
    public class when_metadata_created_for_three_types_with_circular_dependency_regestered_in_different_order : _circular_context
    {
        Because of = () =>
        {
            circularException = Catch.Exception(() =>
            {
                Uniform.Temp.Metadata.DatabaseMetadata.Create(config => config
                    .AddDocumentType<Country>()
                    .AddDocumentType<School>()
                    .AddDocumentType<District>()
                );
            });
        };

        It should_throw_circular_exception = () =>
            circularException.ShouldBeOfType<CircularDependencyNotSupportedException>();

        private static Exception circularException;
    }
}