using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Uniform.Tests.Specs.queries.simple
{
    public class when_executing_count : _simple_context
    {
        Because of = () =>
        {
            query = from u in users.AsQueryable()
                where u.UserName == "Tom"
                select u;

            result = query.Count();
        };

        It should_find_1_item = () =>
            result.ShouldEqual(1);

        private static IQueryable<User> query;
        private static Int32 result;
    }
}