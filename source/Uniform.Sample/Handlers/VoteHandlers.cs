using System;
using Uniform.Sample.Common.Dispatching;
using Uniform.Sample.Events;

namespace Uniform.Sample.Handlers
{
    public class VoteHandlers : IMessageHandler<VoteAdded>
    {
        private readonly MyDatabase _db;

        public VoteHandlers(MyDatabase db)
        {
            _db = db;
        }

        public void Handle(VoteAdded message)
        {
            _db.Votes.Save(message.VoteId, vote =>
            {
                vote.VoteId = message.VoteId;
                vote.UserId = message.UserId;
                vote.Content = message.Content;
                vote.UserDocument = _db.Users.GetById(message.UserId);
            });
        }
    }
}