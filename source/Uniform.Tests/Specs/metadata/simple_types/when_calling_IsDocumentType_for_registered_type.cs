using System;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.simple_types
{
    public class when_calling_IsDocumentType_for_registered_type : _simple_types_context
    {
        Because of = () =>
            documentType = metadata.IsDocumentType(typeof (User));

        It should_be_a_document_type = () =>
            documentType.ShouldBeTrue();

        private static Boolean documentType;
    }
}