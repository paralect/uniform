using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Uniform.Common.Dispatching;
using Uniform.Documents;
using Uniform.Events;
using Uniform.Mongodb;

namespace Uniform.Sample
{
    public class Program
    {
        public static object QU(IQueryable<BsonDocument> q)
        {
            return from i in q
                     select i.GetElement(1);
        }

        public static void Main(string[] args)
        {
            var metadata = new DatabaseMetadata(new List<Type> { typeof(UserDocument), typeof(QuestionDocument), typeof(CommentDocument)});
            metadata.Analyze();
            
            

            var repo = new MongoRepository("mongodb://localhost:27017/local");
            
            
            var a = repo.Test.AsQueryable();

            /*
            var n = (IQueryable<BsonDocument>) repo;
            
            var am = from i in a
                     select i.GetElement(1);

            var m = from i in a
                select i.GetElement(1);
            */
            

            var events = new List<Object>
            {
                new UserCreated("user/1", "Tom", "It's me"),
                new UserCreated("user/2", "John", "Hello!"),
                new QuestionCreated("question/1", "user/1", "Who are you?"),
                new QuestionCreated("question/2", "user/2", "And you?"),
                new UserNameChanged("user/2", "Super John"),
                new QuestionCreated("question/3", "user/2", "How are you?"),
                new QuestionUpdated("question/3", "user/2", "Updated question. How are you?"),
                new CommentAdded("comment/1", "user/1", "question/3", "My first comment!"),
                new UserNameChanged("user/2", "Fucking Tonny"),
            };

            /*
            for (int i = 0; i < 20000; i++)
            {
                events.Add(new UserCreated("user/1", "Tom", "It's me"));
                events.Add(new UserCreated("user/2", "John", "Hello!"));
                events.Add(new QuestionCreated("question/1", "user/1", "Who are you?"));
                events.Add(new QuestionCreated("question/2", "user/2", "And you?"));
                events.Add(new UserNameChanged("user/2", "Super John"));
                events.Add(new QuestionCreated("question/3", "user/2", "How are you?"));
                events.Add(new QuestionUpdated("question/3", "user/2", "Updated question. How are you?"));
                events.Add(new CommentAdded("comment/1", "user/1", "question/3", "My first comment!"));
            }*/

            var instance = new MongodbDatabase("mongodb://localhost:27017/local", metadata);
            //var instance = new InMemoryDatabase(metadata);

            var container = new UnityContainer();
            container.RegisterInstance(repo);
            container.RegisterInstance<IDatabase>(instance);

            var dispatcher = Dispatcher.Create(builder => builder
                .SetServiceLocator(new UnityServiceLocator(container))
                .AddHandlers(typeof(UserCreated).Assembly)
            );

            foreach (var evnt in events)
            {
                dispatcher.Dispatch(evnt);
            }

        }
    }
}
