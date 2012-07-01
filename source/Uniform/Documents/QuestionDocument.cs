using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Documents
{
    public class QuestionDocument
    {
        [BsonId]
        public String QuestionId { get; set; }
        public String UserId { get; set; }
        public String Question { get; set; }

        public UserDocument UserDocument { get; set; }
    }
}