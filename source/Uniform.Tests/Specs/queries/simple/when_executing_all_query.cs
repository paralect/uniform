using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Uniform.Tests.Specs.queries.simple
{
    public class when_executing_all_query : _simple_context
    {
        Because of = () =>
        {
            query = from u in users.AsQueryable()
                    select u;

            result = query.ToList();
        };

        It should_have_3_items = () =>
            result.Count.ShouldEqual(3);

        private static IQueryable<User> query;
        private static List<User> result;
    }
}