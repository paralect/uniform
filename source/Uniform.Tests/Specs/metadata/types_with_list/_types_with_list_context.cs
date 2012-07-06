using System;
using System.Collections.Generic;
using Machine.Specifications;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Tests.Specs.metadata.types_with_list
{
    public class _types_with_list_context
    {
        Establish context = () =>
        {
            metadata = DatabaseMetadata.Create(config => config
                .AddDocumentType<User>()
                .AddDocumentType<Student>()
                .AddDocumentType<School>()
            );
        };

        public static DatabaseMetadata metadata;
    }

    public class User
    {
        [BsonId]
        public String UserId { get; set; }
        public Student Student { get; set; }
    }

    [Collection("students_collection")]
    public class Student
    {
        [BsonId]
        public String StudentId { get; set; }
        public String Name { get; set; }
        public List<School> Schools { get; set; }
    }

    public class School
    {
        [BsonId]
        public String SchoolId { get; set; }
        public Int32 Year { get; set; }
    }
}