using System;
using Machine.Specifications;
using Uniform.Exceptions;

namespace Uniform.Tests.Specs.metadata.circular
{
    public class when_metadata_created_for_three_types_with_circular_dependency : _circular_context
    {
        Because of = () =>
        {
            circularException = Catch.Exception(() =>
            {
                DatabaseMetadata.Create(config => config
                    .AddDocumentType<District>()
                    .AddDocumentType<School>()
                    .AddDocumentType<Country>()
                );
            });
        };

        It should_throw_circular_exception = () =>
            circularException.ShouldBeOfType<CircularDependencyNotSupportedException>();

        private static Exception circularException;
    }
}