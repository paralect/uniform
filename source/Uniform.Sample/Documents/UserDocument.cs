using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Sample.Documents
{
    [Collection("users")]
    public class UserDocument
    {
        [BsonId]
        public String UserId { get; set; }
        public String UserName { get; set; }
        public String About { get; set; }
    }
}