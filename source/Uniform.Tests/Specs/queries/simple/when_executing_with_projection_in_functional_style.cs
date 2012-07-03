using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Uniform.Tests.Specs.queries.simple
{
    public class when_executing_with_projection_in_functional_style : _simple_context
    {
        Because of = () =>
        {
            var query = users.AsQueryable()
                .Where(u => u.UserName == "Tom")
                .Where(u => u.UserName != "Masha")
                .Select(u => new { UserName = u.UserName });

            result = query.ToList()
                .Select(r => r.UserName).ToList();
        };

        It should_find_1_item = () =>
            result.Count.ShouldEqual(1);

        It should_have_find_correct_item = () =>
            result[0].ShouldEqual("Tom");

        private static List<string> result;
    }
}