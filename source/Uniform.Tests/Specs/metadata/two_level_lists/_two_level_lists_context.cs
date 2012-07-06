using System;
using System.Collections.Generic;
using Machine.Specifications;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Tests.Specs.metadata.two_level_lists
{
    public class _two_level_lists_context
    {
    }

    public class User
    {
        [BsonId]
        public String UserId { get; set; }
        public List<Student> Students { get; set; }
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
        public List<District> Districts { get; set; }
    }

    public class District
    {
        [BsonId]
        public String DistrictId { get; set; }
        public String Country { get; set; }
    }
}