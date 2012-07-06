using Uniform.Sample.Common.Dispatching;
using Uniform.Sample.Events;

namespace Uniform.Sample.Handlers
{
    public class CommentHandlers : IMessageHandler<CommentAdded>
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
   }
}