using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.DataAnnotations;

namespace Uniform.Sample.Temp
{
    [Collection("averages")]
    [Alias("averages")]
    public class AverageDocument
    {
        [BsonId]
        public String Id { get; set; }

        [StringLength(1000)]
        public String Name { get; set; }
        public String Hello { get; set; }
        public Int32 Year { get; set; }
        public String Comments { get; set; }
        public String Additional { get; set; }

        [Index(false)]
        public String AnotherId { get; set; }

        public AverageDocument()
        {
        }
    }

    public class Selected
    {
        public String AverageId { get; set; }
        public String AnotherId { get; set; }
    }

    public class AllTogether
    {
        public String AId { get; set; }
        public String AName { get; set; }
        public String AHello { get; set; }
        public Int32 AYear { get; set; }
        public String AComments { get; set; }
        public String AAdditional { get; set; }
        public String AAnotherId { get; set; }

        public String BId { get; set; }
        public String BName { get; set; }
        public String BHello { get; set; }
        public Int32  BYear { get; set; }
        public String BComments { get; set; }
        public String BAdditional { get; set; }
    }
}