using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.DataAnnotations;

namespace Uniform.Sample.Temp
{
    [Collection("anothers")]
    [Alias("anothers")]
    public class AnotherDocument
    {
        [BsonId]
        public String Id { get; set; }

        [StringLength(1000)]
        public String Name { get; set; }
        public String Hello { get; set; }
        public Int32 Year { get; set; }
        public String Comments { get; set; }

        [StringLength(1000)]
        public String Additional { get; set; }

        public AnotherDocument()
        {

        }         
    }
}