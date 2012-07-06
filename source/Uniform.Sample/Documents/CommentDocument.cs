using System;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.Attributes;

namespace Uniform.Sample.Documents
{
    [Collection("comments")]
    public class CommentDocument
    {
        [BsonId]
        public String CommentId { get; set; }
        public String UserId { get; set; }
        public String QuestionId { get; set; }
        public String Content { get; set; }

        public QuestionDocument QuestionDocument { get; set; }
    }
}