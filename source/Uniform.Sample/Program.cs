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
using Uniform.Sample.Handlers;

namespace Uniform.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 1. Create database metadata
            var metadata = DatabaseMetadata.Create(config => config
                .AddDocumentType<UserDocument>()
                .AddDocumentType<QuestionDocument>()
                .AddDocumentType<CommentDocument>()
                .AddDocumentType<VoteDocument>()
            );

            // 2. Create database. Choose euther in-memory or mongodb.
            var mongodbDatabase = new MongodbDatabase("mongodb://localhost:27017/local", metadata);
            var inMemoryDatabase = new InMemoryDatabase(metadata);

            // 3. Optional.
            RunViewModelRegeneration(inMemoryDatabase);

            // 4. Flush to MongoDB
            FlushToMongoDb(inMemoryDatabase, "mongodb://localhost:27017/local");

        }

        public static void RunViewModelRegeneration(IDatabase database)
        {
            Console.Write("Creating list of events in memory... ");

            var events = new List<Object>();
            for (int i = 0; i < 100000; i++)
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

            Console.WriteLine("Done. Managed memory used: {0:n0} mb.", GC.GetTotalMemory(false) / 1024 / 1024);

            var container = new UnityContainer();
            container.RegisterInstance<IDatabase>(database);

            var dispatcher = Dispatcher.Create(builder => builder
                .SetServiceLocator(new UnityServiceLocator(container))
                .AddHandlers(typeof(UserCreated).Assembly)
            );

            Console.WriteLine("Regeneration started...");
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < events.Count; i++)
            {
                if (i % 100000 == 0 && i != 0)
                    Console.WriteLine("{0:n0} events processed.", i);

                var evnt = events[i];
                dispatcher.Dispatch(evnt);
            }
            stopwatch.Stop();

            Console.WriteLine("Done in {0:n0} ms. {1:n0} events processed.", stopwatch.ElapsedMilliseconds, events.Count);
            Console.ReadKey();            
        }

        public static void FlushToMongoDb(InMemoryDatabase inMemoryDatabase, String connectionString)
        {
            Console.WriteLine("Flushing started...");
            var stopwatch = Stopwatch.StartNew();
            
            new MongodbFlusher(inMemoryDatabase, connectionString)
                .Flush();

            stopwatch.Stop();
            Console.WriteLine("Done in {0:n0} ms.", stopwatch.ElapsedMilliseconds);
            Console.ReadKey();            
        }
    }
}
