using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Uniform.Tests.Specs.updaters.inner_lists
{
    public class when_updating_student : _inner_lists_context
    {
        Because of = () =>
        {
            var path = new List<PropertyInfo>();
            path.Add(typeof(User).GetProperty("Student"));

            updater.Update(user, path, new Student
            {
                StudentId = "student2",
                Name = "Bill",
            });
        };

        It name_for_student2_should_be_updated = () =>
            user.Student[1].Name.ShouldEqual("Bill");

        It name_for_student1_should_not_be_updated = () =>
            user.Student[0].Name.ShouldEqual("Tom");
    }
}