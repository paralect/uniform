using System.Collections.Generic;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.simple_types
{
    public class when_searching_for_dependents_of_user : _simple_types_context
    {
        Because of = () =>
            dependences = metadata.GetDependents(typeof (User));

        It should_have_only_one_type_that_depends_on_student = () =>
            dependences.Count.ShouldEqual(0);

        private static List<DependentDocumentMetadata> dependences;
    }
}