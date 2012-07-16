using System.Collections.Generic;
using Uniform.Sample.Common.Dispatching;
using Uniform.Sample.Events;

namespace Uniform.Sample.Handlers
{
    public class UserHandlers : IMessageHandler<UserCreated>, IMessageHandler<UserNameChanged>
    {
        private readonly MyDatabase _db;

        public UserHandlers(MyDatabase db)
        {
            _db = db;
        }

        public void Handle(UserCreated message)
        {
            _db.Users.Save(message.UserId, user =>
            {
                user.UserId = message.UserId;
                user.UserName = message.UserName;
                user.About = message.About;
                user.Ids = new List<string> { "user,/,1,", ",u,s,e,r,2,", "\\,,\\,User3/4", "User/aga" };
            });
        }

        public void Handle(UserNameChanged message)
        {
            _db.Users.Update(message.UserId, user =>
            {
                user.UserName = message.UserName;
            });
        }
    }
}