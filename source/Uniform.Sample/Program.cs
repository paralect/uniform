using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Uniform.InMemory;
using Uniform.Mongodb;
using Uniform.Sample.Common.Dispatching;
using Uniform.Sample.Documents;
using Uniform.Sample.Events;

namespace Uniform.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var metadata = DatabaseMetadata.Create(config => config
                .AddDocumentType<UserDocument>()
                .AddDocumentType<QuestionDocument>()
                .AddDocumentType<CommentDocument>()
                .AddDocumentType<VoteDocument>()
            );

            var repo = new MongoRepository("mongodb://localhost:27017/local");

            var events = new List<Object>();
            for (int i = 0; i < 1000; i++)
            {
                var userId = String.Format("user/{0}", i);
                var question1 = String.Format("user/{0}/question/{1}", i, 1);
                var question2 = String.Format("user/{0}/question/{1}", i, 2);
                var question3 = String.Format("user/{0}/question/{1}", i, 3);
                var commentId = String.Format("user/{0}/comment/{1}", i, 1);
                var voteId = String.Format("user/{0}/comment/{1}/vote/{2}", i, 1, 1);

                events.Add(new UserCreated(userId, "Tom", "It's me"));
                events.Add(new QuestionCreated(question1, userId, "Who are you?"));
                events.Add(new QuestionCreated(question2, userId, "And you?"));
                events.Add(new UserNameChanged(userId, "Super John"));
                events.Add(new QuestionCreated(question3, userId, "How are you?"));
                events.Add(new QuestionUpdated(question3, userId, "Updated question. How are you?"));
                events.Add(new CommentAdded(commentId, userId, question3, "My first comment!"));
                events.Add(new VoteAdded(voteId, commentId, userId, "Nice comment!"));
                events.Add(new UserNameChanged(userId, "Upgraded Tonny"));
                events.Add(new UserNameChanged(userId, "Final Tonny"));
            }

            var instance = new MongodbDatabase("mongodb://localhost:27017/local", metadata);
//            var instance = new InMemoryDatabase(metadata);

            var container = new UnityContainer();
            container.RegisterInstance(repo);
            container.RegisterInstance<IDatabase>(instance);

            var dispatcher = Dispatcher.Create(builder => builder
                .SetServiceLocator(new UnityServiceLocator(container))
                .AddHandlers(typeof(UserCreated).Assembly)
            );

            Console.WriteLine("Started.");
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < events.Count; i++)
            {
                if (i % 100000 == 0)
                    Console.WriteLine(i);

                var evnt = events[i];
                dispatcher.Dispatch(evnt);
            }
            stopwatch.Stop();
            Console.WriteLine("Done in {0} ms", stopwatch.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
