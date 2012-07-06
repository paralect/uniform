using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Uniform.Common.Dispatching;
using Uniform.Documents;
using Uniform.Events;
using Uniform.InMemory;
using Uniform.Mongodb;
using Uniform.Sample.Documents;

namespace Uniform.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var metadata = new DatabaseMetadata(new List<Type> { typeof(UserDocument), typeof(QuestionDocument), typeof(CommentDocument)});
            metadata.Analyze();
            
            var repo = new MongoRepository("mongodb://localhost:27017/local");

            var events = new List<Object>();
            /*
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
            };*/

            for (int i = 0; i < 100000; i++)
            {
                var userId = String.Format("user/{0}", i);
                var question1 = String.Format("user/{0}/question/{1}", i, 1);
                var question2 = String.Format("user/{0}/question/{1}", i, 2);
                var question3 = String.Format("user/{0}/question/{1}", i, 3);
                var commentId = String.Format("user/{0}/comment/{1}", i, 1);

                events.Add(new UserCreated(userId, "Tom", "It's me"));
                events.Add(new QuestionCreated(question1, userId, "Who are you?"));
                events.Add(new QuestionCreated(question2, userId, "And you?"));
                events.Add(new UserNameChanged(userId, "Super John"));
                events.Add(new QuestionCreated(question3, userId, "How are you?"));
                events.Add(new QuestionUpdated(question3, userId, "Updated question. How are you?"));
                events.Add(new CommentAdded(commentId, userId, question3, "My first comment!"));
                events.Add(new UserNameChanged(userId, "Upgraded Tonny"));
                events.Add(new UserNameChanged(userId, "Another Tonny"));
                events.Add(new UserNameChanged(userId, "Final Tonny"));
            }

            //var instance = new MongodbDatabase("mongodb://localhost:27017/local", metadata);
            var instance = new InMemoryDatabase(metadata);

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
