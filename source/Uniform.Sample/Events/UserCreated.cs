using System;
using ProtoBuf;

namespace Uniform.Sample.Events
{
    [ProtoContract]
    public class UserCreated
    {
        [ProtoMember(1)]
        public String UserId { get; set; }
        [ProtoMember(2)]
        public String UserName { get; set; }
        [ProtoMember(3)]
        public String About { get; set; }
        [ProtoMember(4)]
        public String Blob { get; set; }

        public UserCreated() { }
        public UserCreated(string userId, string userName, string about) : this()
        {
            UserId = userId;
            UserName = userName;
            About = about;

            Blob = "Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). " +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." +
                "Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях." + _random.Next();
        }

        private static Random _random = new Random();
    }

    [ProtoContract]
    public class UserNameChanged
    {
        [ProtoMember(1)]
        public String UserId { get; set; }
        [ProtoMember(2)]
        public String UserName { get; set; }

        public UserNameChanged() { }
        public UserNameChanged(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }

    [ProtoContract]
    public class QuestionCreated
    {
        [ProtoMember(1)]
        public String QuestionId { get; set; }
        [ProtoMember(2)]
        public String UserId { get; set; }
        [ProtoMember(3)]
        public String Question { get; set; }

        public QuestionCreated() { }
        public QuestionCreated(string questionId, string userId, string question)
        {
            QuestionId = questionId;
            UserId = userId;
            Question = question;
        }
    }

    [ProtoContract]
    public class QuestionUpdated
    {
        [ProtoMember(1)]
        public String QuestionId { get; set; }
        [ProtoMember(2)]
        public String UserId { get; set; }
        [ProtoMember(3)]
        public String Question { get; set; }

        public QuestionUpdated() { }
        public QuestionUpdated(string questionId, string userId, string question)
        {
            QuestionId = questionId;
            UserId = userId;
            Question = question;
        }
    }

    [ProtoContract]
    public class CommentAdded
    {
        [ProtoMember(1)]
        public String CommentId { get; set; }
        [ProtoMember(2)]
        public String UserId { get; set; }
        [ProtoMember(3)]
        public String QuestionId { get; set; }
        [ProtoMember(4)]
        public String Content { get; set; }

        public CommentAdded() { }
        public CommentAdded(string commentId, string userId, string questionId, string content)
        {
            CommentId = commentId;
            UserId = userId;
            Content = content;
            QuestionId = questionId;
        }
    }

    [ProtoContract]
    public class VoteAdded
    {
        [ProtoMember(1)]
        public String VoteId { get; set; }
        [ProtoMember(2)]
        public String CommentId { get; set; }
        [ProtoMember(3)]
        public String UserId { get; set; }
        [ProtoMember(4)]
        public String Content { get; set; }

        public VoteAdded() { }
        public VoteAdded(string voteId, string commentId, string userId, string content)
        {
            VoteId = voteId;
            CommentId = commentId;
            UserId = userId;
            Content = content;
        }
    }




}