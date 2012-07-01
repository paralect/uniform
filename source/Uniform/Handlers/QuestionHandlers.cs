using System;
using Uniform.Common.Dispatching;
using Uniform.Documents;
using Uniform.Events;

namespace Uniform.Handlers
{
    public class QuestionHandlers : IMessageHandler<QuestionCreated>, IMessageHandler<QuestionUpdated>
    {
        private readonly MyDatabase _db;

        public QuestionHandlers(MyDatabase db)
        {
            _db = db;
        }


        public void Handle(QuestionCreated message)
        {
            _db.Questions.Save(message.QuestionId, question =>
            {
                question.QuestionId = message.QuestionId;
                question.Question = message.Question;
                question.UserId = message.UserId;
                question.UserDocument = _db.Users.GetById(message.UserId);
            });
            
        }

        public void Handle(QuestionUpdated message)
        {
            _db.Questions.Update(message.QuestionId, question =>
            {
                question.Question = message.Question;
            });
        }
    }
}