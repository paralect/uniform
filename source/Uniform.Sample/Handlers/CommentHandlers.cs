using Uniform.Sample.Common.Dispatching;
using Uniform.Sample.Documents;
using Uniform.Sample.Events;

namespace Uniform.Sample.Handlers
{
    public class CommentHandlers : IMessageHandler<CommentAdded>, IMessageHandler<VoteAdded>
    {
        private readonly MyDatabase _db;

        public CommentHandlers(MyDatabase db)
        {
            _db = db;
        }

        public void Handle(CommentAdded message)
        {
            _db.Comments.Save(message.CommentId, comment =>
            {
                comment.CommentId = message.CommentId;
                comment.Content = message.Content;
                comment.QuestionId = message.QuestionId;
                comment.UserId = message.UserId;
                comment.QuestionDocument = _db.Questions.GetById(message.QuestionId);
            });

            var user = _db.Users.GetById(message.UserId);
            user.About = "Hello";
        }

        public void Handle(VoteAdded message)
        {
            _db.Comments.Update(message.CommentId, comment =>
            {
                comment.Votes.Add(new VoteDocument()
                {
                    Content = message.Content,
                    UserId = message.UserId,
                    VoteId = message.VoteId,
                    UserDocument = _db.Users.GetById(message.UserId)
                });
            });
        }
    }
}