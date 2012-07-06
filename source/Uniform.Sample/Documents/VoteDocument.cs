using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Sample.Documents
{
    [Collection("votes")]
    public class VoteDocument
    {
        [BsonId]
        public String VoteId { get; set; }
        public String UserId { get; set; }
        public String Content { get; set; }

        public UserDocument UserDocument { get; set; }
    }
}