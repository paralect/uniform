using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Uniform.Tests.Specs.updaters.case1
{
    public class when_updating_student_with_invalid_key : _case1_context
    {
        Because of = () =>
        {
            var path = new List<PropertyInfo>();
            path.Add(typeof(User).GetProperty("Student"));

            updater.Update(user, path, "student_invalid_key", new Student()
            {
                StudentId = "student_new",
                School = null
            });
        };

        It should_have_null_school_now = () =>
            user.Student.School.ShouldNotBeNull();

        It id_should_be_updated = () =>
            user.Student.StudentId.ShouldEqual("student1");
    }
}