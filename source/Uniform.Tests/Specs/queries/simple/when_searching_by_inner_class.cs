using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Uniform.Tests.Specs.queries.simple
{
    public class when_searching_by_inner_class : _simple_context
    {
        Because of = () =>
        {
            query = from u in users.AsQueryable()
                where u.Student.Name == "Super Student"
                select u;

            result = query.ToList();
        };

        It should_find_1_item = () =>
            result.Count.ShouldEqual(1);

        It should_have_find_correct_item = () =>
            result[0].Student.Name.ShouldEqual("Super Student");

        private static IUniformable<User> query;
        private static List<User> result;
    }
}