using System;
using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;

namespace Uniform.Tests.Specs.updaters.lists
{
    public class when_updating_student2 : _lists_context
    {
        Because of = () =>
        {
            var path = new List<PropertyInfo>();
            path.Add(typeof(User).GetProperty("Student"));

            updater.Update(user, path, "student2", new Student()
            {
                StudentId = "student_new",
                School = null
            });
        };
    }
}