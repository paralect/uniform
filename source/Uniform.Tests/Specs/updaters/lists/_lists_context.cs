using System;
using System.Collections.Generic;
using Machine.Specifications;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.Mongodb;

namespace Uniform.Tests.Specs.updaters.lists
{
    public class _lists_context
    {
        Establish context = () =>
        {
            var metadata = DatabaseMetadata.Create(config => config
                .AddDocumentType<User>()
                .AddDocumentType<Student>()
                .AddDocumentType<School>()
            );

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
                        School = new School()
                        {
                            SchoolId = "school1",
                            Year = 2011,
                        }
                    },
                    new Student()
                    {
                        StudentId = "student2",
                        Name = "John",
                        School = new School()
                        {
                            SchoolId = "school2",
                            Year = 2012,
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
        public School School { get; set; }
    }

    public class School
    {
        [BsonId]
        public String SchoolId { get; set; }

        public Int32 Year { get; set; }
    }
}