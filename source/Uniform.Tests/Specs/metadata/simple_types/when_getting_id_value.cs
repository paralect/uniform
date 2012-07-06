using System;
using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.simple_types
{
    public class when_getting_id_value : _simple_types_context
    {
        Because of = () =>
        {
            var user = new User();
            user.UserId = "id_value";
            
            id = metadata.GetDocumentId(user);
        };

        It should_have_correct_id = () =>
            id.ShouldEqual("id_value");
            
        private static String id; 
    }
}