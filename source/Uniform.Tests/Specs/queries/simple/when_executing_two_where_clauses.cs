using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Uniform.Tests.Specs.queries.simple
{
    public class when_executing_two_where_clauses : _simple_context
    {
        Because of = () =>
        {
            query = from u in users.AsQueryable()
                where u.UserName == "Tom"
                where u.UserName != "Masha"
                select u;

            result = query.ToList();
        };

        It should_find_1_item = () =>
            result.Count.ShouldEqual(1);

        It should_have_find_correct_item = () =>
            result[0].UserName.ShouldEqual("Tom");

        private static IUniformable<User> query;
        private static List<User> result;
    }
}