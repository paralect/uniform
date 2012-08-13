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
        private readonly UniformDatabase _database;

        public MyDatabase(UniformDatabase database)
        {
            _database = database;
        }

        public IDocumentCollection<UserDocument> Users
        {
            get { return _database.GetCollection<UserDocument>(SampleDatabases.Sql, SampleCollections.Users); }
        }        
        
        public IDocumentCollection<QuestionDocument> Questions
        {
            get { return _database.GetCollection<QuestionDocument>(SampleDatabases.Mongodb, SampleCollections.Questions); }
        }

        public IDocumentCollection<CommentDocument> Comments
        {
            get { return _database.GetCollection<CommentDocument>(SampleDatabases.Sql, SampleCollections.Comments); }
        }

        public IDocumentCollection<VoteDocument> Votes
        {
            get { return _database.GetCollection<VoteDocument>(SampleDatabases.Mongodb, SampleCollections.Votes); }
        }
    }
}