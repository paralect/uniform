using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.simple_types
{
    public class when_searching_for_dependents_of_student : _simple_types_context
    {
        Because of = () =>
            dependences = metadata.GetDependents(typeof (Student));

        It should_have_only_one_type_that_depends_on_student = () =>
            dependences.Count.ShouldEqual(1);

        It should_be_of_type_user = () =>
            dependences[0].DependentDocumentType.ShouldEqual(typeof(User));

        It should_have_correct_number_of_path_items = () =>
            dependences[0].SourceDocumentPath.Count.ShouldEqual(1);

        It should_have_correct_path = () =>
            dependences[0].SourceDocumentPath[0].ShouldEqual(typeof(User).GetProperty("Student"));

        private static List<Uniform.Temp.Metadata.DependentDocumentMetadata> dependences;
    }
}