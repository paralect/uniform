using System;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.simple_types
{
    public class when_calling_IsDocumentType_for_not_registered_type : _simple_types_context
    {
        Because of = () =>
            documentType = metadata.IsDocumentType(typeof (String));

        It should_not_be_a_document_type = () =>
            documentType.ShouldBeFalse();

        private static Boolean documentType;
    }
}