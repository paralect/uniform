using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Uniform.Tests.Specs.updaters.lists
{
    public class when_updating_school_1 : _lists_context
    {
        Because of = () =>
        {
            var path = new List<PropertyInfo>();
            path.Add(typeof(User).GetProperty("Student"));
            path.Add(typeof(Student).GetProperty("School"));

            updater.Update(user, path, new School
            {
                SchoolId = "school1",
                Year = 999,
            });
        };

        It should_aga = () =>
            user.Student[0].School.Year.ShouldEqual(999);

    }
}