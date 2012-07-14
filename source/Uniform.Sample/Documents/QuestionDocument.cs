using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.DataAnnotations;

namespace Uniform.Sample.Documents
{
    [Document(SampleDatabases.Mongodb, "questions")]
    [Alias("questions")]
    public class QuestionDocument
    {
        [BsonId]
        public String QuestionId { get; set; }
        public String UserId { get; set; }
        public String Question { get; set; }

        [StringLength(6000)]
        public UserDocument UserDocument { get; set; }
    }
}