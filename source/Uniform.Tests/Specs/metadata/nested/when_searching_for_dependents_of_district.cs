using System;
using System.Collections.Generic;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.nested
{
    public class when_searching_for_dependents_of_district : _nested_context
    {
        Because of = () =>
            dependences = metadata.GetDependents(typeof (District));

        It should_have_only_one_type_that_depends_on_district = () =>
            dependences.Count.ShouldEqual(3);

        It should_has_correct_dependents_types = () =>
        {
            dependences[0].DependentDocumentType.ShouldEqual(typeof(User));
            dependences[1].DependentDocumentType.ShouldEqual(typeof(Student));
            dependences[2].DependentDocumentType.ShouldEqual(typeof(School));
        };
            
        It should_have_correct_number_of_path_items_for_user = () =>
            dependences[0].SourceDocumentPath.Count.ShouldEqual(3);

        It should_have_correct_path_for_user = () =>
        {
            dependences[0].SourceDocumentPath[0].ShouldEqual(typeof(User).GetProperty("Students"));
            dependences[0].SourceDocumentPath[1].ShouldEqual(typeof(Student).GetProperty("School"));
            dependences[0].SourceDocumentPath[2].ShouldEqual(typeof(School).GetProperty("Districts"));
        };

        It should_have_correct_number_of_path_items_for_student = () =>
            dependences[1].SourceDocumentPath.Count.ShouldEqual(2);

        It should_have_correct_path_for_student = () =>
        {
            dependences[1].SourceDocumentPath[0].ShouldEqual(typeof(Student).GetProperty("School"));
            dependences[1].SourceDocumentPath[1].ShouldEqual(typeof(School).GetProperty("Districts"));
        };

        It should_have_correct_number_of_path_items_for_school = () =>
            dependences[1].SourceDocumentPath.Count.ShouldEqual(2);

        It should_have_correct_path_for_school = () =>
        {
            dependences[2].SourceDocumentPath[0].ShouldEqual(typeof(School).GetProperty("Districts"));
        };
            

        private static List<DependentDocumentMetadata> dependences;
    }}