using System;
using System.Collections.Generic;
using Machine.Specifications;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.Mongodb;

namespace Uniform.Tests.Specs.updaters.inner_lists
{
    public class _inner_lists_context
    {
        Establish context = () =>
        {
            var metadata = new DatabaseMetadata(new List<Type>() { typeof(User), typeof(Student), typeof(School) });
            updater = new Updater(metadata);

            user = new User()
            {
                UserId = "user1",
                Student = new List<Student>
                {
                    new Student()
                    {
                        StudentId = "student1",
                        Name = "Tom",
                        School = new List<School>
                        {
                            new School { SchoolId = "school1", Year = 2011 },
                            new School { SchoolId = "school2", Year = 2012 }
                        }
                    },
                    new Student()
                    {
                        StudentId = "student2",
                        Name = "John",
                        School = new List<School>
                        {
                            new School { SchoolId = "school3", Year = 2013 },
                            new School { SchoolId = "school4", Year = 2014 },
                            new School { SchoolId = "school5", Year = 2015 },
                            new School { SchoolId = "school6", Year = 2016 },
                        }
                    }                    
                }
            };
        };

        public static Updater updater;
        public static User user;
    }
    public class User
    {
        [BsonId]
        public String UserId { get; set; }
        public List<Student> Student { get; set; }
    }

    public class Student
    {
        [BsonId]
        public String StudentId { get; set; }
        public String Name { get; set; }
        public List<School> School { get; set; }
    }

    public class School
    {
        [BsonId]
        public String SchoolId { get; set; }

        public Int32 Year { get; set; }
    }
}