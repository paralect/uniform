using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Uniform.Tests.Specs.queries.simple
{
    public class when_executing_with_projection : _simple_context
    {
        Because of = () =>
        {
            var query = from u in users.AsQueryable()
                    where u.UserName == "Tom"
                    where u.UserName != "Masha"
                    select new { u.UserName };

            result = query.ToList()
                .Select(r => r.UserName).ToList();
        };

        It should_find_1_item = () =>
            result.Count.ShouldEqual(1);

        It should_have_find_correct_item = () =>
            result[0].ShouldEqual("Tom");

        private static List<String> result;         
    }
}