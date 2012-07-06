using System;

namespace Uniform.Sample.Events
{
    public class UserCreated
    {
        public String UserId { get; set; }
        public String UserName { get; set; }
        public String About { get; set; }

        public UserCreated(string userId, string userName, string about)
        {
            UserId = userId;
            UserName = userName;
            About = about;
        }
    }

    public class UserNameChanged
    {
        public String UserId { get; set; }
        public String UserName { get; set; }

        public UserNameChanged(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }

    public class QuestionCreated
    {
        public String QuestionId { get; set; }
        public String UserId { get; set; }
        public String Question { get; set; }

        public QuestionCreated(string questionId, string userId, string question)
        {
            QuestionId = questionId;
            UserId = userId;
            Question = question;
        }
    }

    public class QuestionUpdated
    {
        public String QuestionId { get; set; }
        public String UserId { get; set; }
        public String Question { get; set; }

        public QuestionUpdated(string questionId, string userId, string question)
        {
            QuestionId = questionId;
            UserId = userId;
            Question = question;
        }
    }

    public class CommentAdded
    {
        public String CommentId { get; set; }
        public String UserId { get; set; }
        public String QuestionId { get; set; }
        public String Content { get; set; }

        public CommentAdded(string commentId, string userId, string questionId, string content)
        {
            CommentId = commentId;
            UserId = userId;
            Content = content;
            QuestionId = questionId;
        }
    }


}