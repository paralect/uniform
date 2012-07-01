using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Documents
{
    public class UserDocument
    {
        [BsonId]
        public String UserId { get; set; }
        public String UserName { get; set; }
        public String About { get; set; } 
    }

    public class QuestionDocument
    {
        [BsonId]
        public String QuestionId { get; set; }
        public String UserId { get; set; }
        public String Question { get; set; }

        public UserDocument UserDocument { get; set; }
    }

    public class CommentDocument
    {
        [BsonId]
        public String CommentId { get; set; }
        public String UserId { get; set; }
        public String QuestionId { get; set; }
        public String Content { get; set; }        
    }
}