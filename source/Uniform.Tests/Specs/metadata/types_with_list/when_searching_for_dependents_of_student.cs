using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.types_with_list
{
    public class when_searching_for_dependents_of_student : _types_with_list_context
    {
        Because of = () =>
            dependences = metadata.GetDependents(typeof(Student));

        It should_have_only_one_type_that_depends_on_student = () =>
            dependences.Count.ShouldEqual(1);

        It should_be_of_type_user = () =>
            dependences[0].DependentDocumentType.ShouldEqual(typeof(User));

        It should_have_correct_number_of_path_items = () =>
            dependences[0].SourceDocumentPath.Count.ShouldEqual(1);

        It should_have_correct_path = () =>
            dependences[0].SourceDocumentPath[0].ShouldEqual(typeof(User).GetProperty("Student"));

        private static List<DependentDocumentMetadata> dependences;
    }
}