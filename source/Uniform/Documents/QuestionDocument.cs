using System;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.Storage.Attributes;

namespace Uniform.Documents
{
    [Collection("questions")]
    public class QuestionDocument
    {
        [BsonId]
        public String QuestionId { get; set; }
        public String UserId { get; set; }
        public String Question { get; set; }

        public UserDocument UserDocument { get; set; }
    }
}