using System;
using Machine.Specifications;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Tests.Specs.metadata.many
{
    public class _many_context
    {
        Establish context = () =>
        {
            metadata = DatabaseMetadata.Create(config => config
                .AddDocumentType<User>()
                .AddDocumentType<Student>()
                .AddDocumentType<School>()
                .AddDocumentType<District>()
            );
        };

        public static DatabaseMetadata metadata;
    }

    public class User
    {
        [BsonId]
        public String UserId { get; set; }
        public Student Student { get; set; }
        public School School { get; set; }
    }

    public class Student
    {
        [BsonId]
        public String StudentId { get; set; }
        public String Name { get; set; }
        public School School { get; set; }
    }

    public class School
    {
        [BsonId]
        public String SchoolId { get; set; }
        public Int32 Year { get; set; }
    }

    public class District
    {
        [BsonId]
        public String DistrictId { get; set; }
        public String Country { get; set; }
    }
}