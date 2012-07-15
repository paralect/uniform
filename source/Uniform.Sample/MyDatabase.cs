using System;
using Uniform.Sample.Documents;

namespace Uniform.Sample
{
    public static class SampleDatabases
    {
        public const String Mongodb = "mongodb";
        public const String Sql = "sql";
    }

    public static class SampleCollections
    {
        public const String Users = "users";
        public const String Questions = "questions";
        public const String Comments = "comments";
        public const String Votes = "votes";
    }

    public class MyDatabase
    {
        private readonly IDatabase _database;

        public MyDatabase(IDatabase database)
        {
            _database = database;
        }

        public ICollection<UserDocument> Users
        {
            get { return _database.GetCollection<UserDocument>(SampleCollections.Users); }
        }        
        
        public ICollection<QuestionDocument> Questions
        {
            get { return _database.GetCollection<QuestionDocument>(SampleCollections.Questions); }
        }

        public ICollection<CommentDocument> Comments
        {
            get { return _database.GetCollection<CommentDocument>(SampleCollections.Comments); }
        }

        public ICollection<VoteDocument> Votes
        {
            get { return _database.GetCollection<VoteDocument>(SampleCollections.Votes); }
        }
    }
}