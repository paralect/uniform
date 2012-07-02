using System;
using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Uniform.Tests.Specs.updaters.case1
{
    public class when_updating_school : _case1_context
    {
        Because of = () =>
        {
            var path = new List<PropertyInfo>();
            path.Add(typeof(User).GetProperty("Student"));
            path.Add(typeof(Student).GetProperty("School"));
            
            updater.Update(user, path, new School
            {
                SchoolId = "school1", 
                Year = 2012
            });
        };

        It year_should_be_updated = () =>
            user.Student.School.Year.ShouldEqual(2012);

        It id_should_be_the_same = () =>
            user.Student.School.SchoolId.ShouldEqual("school1");
    }
}