using System;
using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Uniform.Tests.Specs.updaters.lists
{
    public class when_updating_student_2 : _lists_context
    {
        Because of = () =>
        {
            var path = new List<PropertyInfo>();
            path.Add(typeof(User).GetProperty("Student"));

            updater.Update(user, path, new Student
            {
                StudentId = "student2",
                Name = "Super John",
            });
        };

        It should_have_unchanged_id_at_index_1 = () =>
            user.Student[1].StudentId.ShouldEqual("student2");

        It should_update_student_at_index_1 = () =>
            user.Student[1].Name.ShouldEqual("Super John");

        It should_have_unchanged_id_at_index_0 = () =>
            user.Student[0].StudentId.ShouldEqual("student1");

        It should_not_update_student_at_index_0 = () =>
            user.Student[0].Name.ShouldEqual("Tom");
    }
}