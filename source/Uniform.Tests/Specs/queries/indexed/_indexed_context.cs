using System;
using System.Collections.Generic;
using Machine.Specifications;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.InMemory;

namespace Uniform.Tests.Specs.queries.indexed
{
    public class _indexed_context
    {
        Establish context = () =>
        {
            var metadata = new DatabaseMetadata(new List<Type>() { typeof(User), typeof(Student), typeof(School) });
            db = new InMemoryDatabase(metadata);

            var user1 = new User()
            {
                UserId = "user1",
                UserName = "Tom",
                Student = new Student()
                {
                    StudentId = "student1",
                    Name = "Super Student",
                    School = new School()
                    {
                        SchoolId = "school1",
                        Year = 2012,
                    }
                }
            };

            var user2 = new User()
            {
                UserId = "user2",
                UserName = "Pol",
                Student = new Student(),
            };

            var user3 = new User()
            {
                UserId = "user3",
                UserName = "John",
                Student = new Student(),
            };

            users = db.GetCollection<User>();
            users.Save("user1", user1);
            users.Save("user2", user2);
            users.Save("user3", user3);
            users.Update("user2", user => user.UserName = "Updated Name");
        };

        public static ICollection<User> users;
        public static InMemoryDatabase db;
    }

    public class User : IIndexable<User>
    {
        [BsonId]
        public String UserId { get; set; }

        public String UserName { get; set; }
        public Student Student { get; set; }

        public void DefineIndexes(IndexContext<User> definition)
        {
            definition.Define("first_index", u => u.UserName);
            definition.Define("second_index", u => u.UserId, u => u.Student.Name);
        }
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