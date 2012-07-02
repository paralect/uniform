using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Uniform.Tests.Specs.updaters.simple
{
    public class when_updating_student_with_invalid_key : _simple_context
    {
        Because of = () =>
        {
            var path = new List<PropertyInfo>();
            path.Add(typeof(User).GetProperty("Student"));

            updater.Update(user, path, new Student
            {
                StudentId = "student_invalid_key",
                School = null
            });
        };

        It should_have_null_school_now = () =>
            user.Student.School.ShouldNotBeNull();

        It id_should_be_updated = () =>
            user.Student.StudentId.ShouldEqual("student1");
    }
}