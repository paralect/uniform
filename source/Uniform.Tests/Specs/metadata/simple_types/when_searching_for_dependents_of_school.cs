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

        It should_has_correct_dependents_types = () =>
        {
            dependences[0].DependentDocumentType.ShouldEqual(typeof(User));
            dependences[1].DependentDocumentType.ShouldEqual(typeof(Student));
        };
            
        It should_have_correct_number_of_path_items_for_user = () =>
            dependences[0].SourceDocumentPath.Count.ShouldEqual(2);

        It should_have_correct_path_for_user = () =>
        {
            dependences[0].SourceDocumentPath[0].ShouldEqual(typeof(User).GetProperty("Student"));
            dependences[0].SourceDocumentPath[1].ShouldEqual(typeof(Student).GetProperty("School"));
        };

        It should_have_correct_number_of_path_items_for_student = () =>
            dependences[1].SourceDocumentPath.Count.ShouldEqual(1);

        It should_have_correct_path_for_student = () =>
            dependences[1].SourceDocumentPath[0].ShouldEqual(typeof(Student).GetProperty("School"));

        private static List<Uniform.Temp.Metadata.DependentDocumentMetadata> dependences;
    }
}