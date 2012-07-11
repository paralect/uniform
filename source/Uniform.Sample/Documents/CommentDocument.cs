using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.DataAnnotations;

namespace Uniform.Sample.Documents
{
    [Collection("comments")]
    [Alias("comments")]
    public class CommentDocument
    {
        [BsonId]
        public String CommentId { get; set; }
        public String UserId { get; set; }
        public String QuestionId { get; set; }
        public String Content { get; set; }
        
        [StringLength(6000)]
        public QuestionDocument QuestionDocument { get; set; }

        [StringLength(6000)]
        public List<VoteDocument> Votes { get; set; }

        public CommentDocument()
        {
            Votes = new List<VoteDocument>();
        }
    }
}