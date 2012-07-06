using System.Collections.Generic;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.simple_types
{
    public class when_searching_for_dependents_of_school : _simple_types_context
    {
        Because of = () =>
            dependences = metadata.GetDependents(typeof (School));

        It should_have_only_one_type_that_depends_on_student = () =>
            dependences.Count.ShouldEqual(2);

//        It should_be_of_type_user = () =>
//            dependences[0].DependentDocumentType.ShouldEqual(typeof(User));

        private static List<DependentDocumentMetadata> dependences;
    }
}