using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Uniform.Tests.Specs.queries.simple
{
    public class when_executing_simple_where_clause : _simple_context
    {
        Because of = () =>
        {
            query = from u in users.AsQueryable()
                where u.UserName == "Tom"
                select u;

            result = query.ToList();
        };

        It should_find_1_item = () =>
            result.Count.ShouldEqual(1);

        It should_have_find_correct_item = () =>
            result[0].UserName.ShouldEqual("Tom");

        private static IQueryable<User> query;
        private static List<User> result;
    }
}