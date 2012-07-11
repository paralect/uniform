using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.DataAnnotations;

namespace Uniform.Sample.Documents
{
    [Collection("votes")]
    [Alias("votes")]
    public class VoteDocument
    {
        [BsonId]
        public String VoteId { get; set; }
        public String UserId { get; set; }
        public String Content { get; set; }

        [StringLength(6000)]
        public UserDocument UserDocument { get; set; }
    }
}