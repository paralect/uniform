using System;
using System.Collections.Generic;
using Machine.Specifications;
using MongoDB.Bson.Serialization.Attributes;

namespace Uniform.Tests.Specs.metadata.circular
{
    public class _circular_context
    {
    }

    #region Circular dependency between two types

    public class User
    {
        [BsonId]
        public String UserId { get; set; }
        public Student Student { get; set; }
    }

    public class Student
    {
        [BsonId]
        public String StudentId { get; set; }
        public User User { get; set; }
    }

    #endregion

    #region Circular dependency between three types

    public class School
    {
        [BsonId]
        public String SchoolId { get; set; }
        public District District { get; set; }
    }

    public class District
    {
        [BsonId]
        public String DistrictId { get; set; }
        public List<Country> Country { get; set; }
    }

    public class Country
    {
        [BsonId]
        public String CountryId { get; set; }
        public School School { get; set; }
    }
    
    #endregion 
}