using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProtoBuf;
using Uniform.InMemory;
using Uniform.Mongodb;
using Uniform.Sample.Common;
using Uniform.Sample.Common.Dispatching;
using Uniform.Sample.Documents;
using Uniform.Sample.Events;
using Uniform.Sample.Handlers;
using Uniform.Sql;

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
            var mysqlDatabase = new MySqlDatabase("server=127.0.0.1;Uid=root;Pwd=qwerty;Database=test;", metadata);
            ((MySqlCollection<UserDocument>) mysqlDatabase.GetCollection<UserDocument>()).CreateTable();
            ((MySqlCollection<QuestionDocument>)mysqlDatabase.GetCollection<QuestionDocument>()).CreateTable();
            ((MySqlCollection<CommentDocument>)mysqlDatabase.GetCollection<CommentDocument>()).CreateTable();
            ((MySqlCollection<VoteDocument>)mysqlDatabase.GetCollection<VoteDocument>()).CreateTable();
            

            // 3. Optional.
            RunViewModelRegeneration(mysqlDatabase);

            // 4. Flush to MongoDB
            //FlushToMongoDb(inMemoryDatabase, "mongodb://localhost:27017/local");

        }

        public static void RunViewModelRegeneration(IDatabase database)
        {
            Console.Write("Creating list of events in memory... ");

            var events = new List<Object>();
            for (int i = 0; i < 10; i++)
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

//            SaveEventsToFile(events);
//            var result = LoadEventsFromFile();

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

        public static void SaveEventsToFile(List<Object> events)
        {
            Console.WriteLine();
            var dir = @"c:\tmp\uniform";
            var file = @"data.proto";
            var path = Path.Combine(dir, file);

            Console.WriteLine("Storing {0:n0} events to file {1}", events.Count, path);
            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (File.Exists(path))
                File.Delete(path);

            var serializer = new ProtobufSerializer();

            var stopwatch = Stopwatch.StartNew();
            using(var writeStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            using(var binaryWriter = new BinaryWriter(writeStream))
            {
                foreach (object evnt in events)
                {
                    binaryWriter.Write(_typeToTag[evnt.GetType()]);
                    serializer.Model.SerializeWithLengthPrefix(writeStream, evnt, evnt.GetType(), PrefixStyle.Base128, 0);
                }
                    
            }
            stopwatch.Stop();
            Console.WriteLine("All events stored in {0:n0} ms", stopwatch.ElapsedMilliseconds);
        }

        public static List<Object> LoadEventsFromFile()
        {
            Console.ReadKey();
            List<Object> result = new List<object>();

            var dir = @"c:\tmp\uniform";
            var file = @"data.proto";
            var path = Path.Combine(dir, file);
            Console.WriteLine("Loading events from file {0}...", path);

            var serializer = new ProtobufSerializer();

            var stopwatch = Stopwatch.StartNew();
            using (var writeStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var binaryReader = new BinaryReader(writeStream))
            {
                while (true)
                {
                    try
                    {
                        var tag = binaryReader.ReadInt32();
                        var obj = serializer.Model.DeserializeWithLengthPrefix(writeStream, null, _tagToType[tag], PrefixStyle.Base128, 0);

                        if (obj == null)
                            break;

                        result.Add(obj);
                    }
                    catch (EndOfStreamException exception)
                    {
                        // planned exception, better performance than checking for the end of stream
                        break;
                    }
                    
                }
                
                
            }
            stopwatch.Stop();
            Console.WriteLine("{0:n0} events loaded and deserialized in {1:n0} ms", result.Count, stopwatch.ElapsedMilliseconds);

            return result;
        }

        #region Plumbing (not important)

        public static Dictionary<Type, Int32> _typeToTag = new Dictionary<Type, int>
        {
            {typeof(UserCreated), 1},
            {typeof(UserNameChanged), 2},
            {typeof(QuestionCreated), 3},
            {typeof(QuestionUpdated), 4},
            {typeof(CommentAdded), 5},
            {typeof(VoteAdded), 6},
        };

        public static Dictionary<Int32, Type> _tagToType = new Dictionary<int, Type>
        {
            {1, typeof(UserCreated)},
            {2, typeof(UserNameChanged)},
            {3, typeof(QuestionCreated)},
            {4, typeof(QuestionUpdated)},
            {5, typeof(CommentAdded)},
            {6, typeof(VoteAdded)},
        };

        #endregion
    }
}
