using System;
using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Uniform.Tests.Specs.updaters.inner_lists
{
    public class when_updating_school : _inner_lists_context
    {
        Because of = () =>
        {
            var path = new List<PropertyInfo>();
            path.Add(typeof(User).GetProperty("Student"));
            path.Add(typeof(Student).GetProperty("School"));

            updater.Update(user, path, new School
            {
                SchoolId = "school3",
                Year = 888,
            });
        };

        It year_for_school3_should_be_updated = () =>
            user.Student[1].School[0].Year.ShouldEqual(888);
    }
}