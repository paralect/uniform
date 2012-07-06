using Uniform.Sample.Documents;

namespace Uniform.Sample
{
    public class MyDatabase
    {
        private readonly IDatabase _database;

        public MyDatabase(IDatabase database)
        {
            _database = database;
        }

        public ICollection<UserDocument> Users
        {
            get { return _database.GetCollection<UserDocument>(); }
        }        
        
        public ICollection<QuestionDocument> Questions
        {
            get { return _database.GetCollection<QuestionDocument>(); }
        }

        public ICollection<CommentDocument> Comments
        {
            get { return _database.GetCollection<CommentDocument>(); }
        }

        public ICollection<VoteDocument> Votes
        {
            get { return _database.GetCollection<VoteDocument>(); }
        }


    }
}