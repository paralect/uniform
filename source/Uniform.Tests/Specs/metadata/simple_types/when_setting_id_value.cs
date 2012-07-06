using Machine.Specifications;

namespace Uniform.Tests.Specs.metadata.simple_types
{
    public class when_setting_id_value : _simple_types_context
    {
        Because of = () =>
        {
            user = new User();
            metadata.SetDocumentId(user, "new_value");
        };

        It should_have_correct_id = () =>
            user.UserId.ShouldEqual("new_value");
            
        private static User user; 
    }
}