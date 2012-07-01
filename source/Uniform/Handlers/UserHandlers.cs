using System;
using Uniform.Common.Dispatching;
using Uniform.Documents;
using Uniform.Events;

namespace Uniform.Handlers
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