using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Documents
{
    public class CommentDocument
    {
        [BsonId]
        public String CommentId { get; set; }
        public String UserId { get; set; }
        public String QuestionId { get; set; }
        public String Content { get; set; }        
    }
}