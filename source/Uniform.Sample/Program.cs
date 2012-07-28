using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProtoBuf;
using Uniform.AdoNet;
using Uniform.InMemory;
using Uniform.Mongodb;
using Uniform.Sample.Common;
using Uniform.Sample.Common.Dispatching;
using Uniform.Sample.Documents;
using Uniform.Sample.Events;
using Uniform.Sample.Handlers;
using Uniform.Sample.Temp;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;

namespace Uniform.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 1. Create databases.
            var mongodbDatabase = new MongodbDatabase("mongodb://localhost:27017/local");
            //var mysqlDatabase = new AdoNetDatabase("server=127.0.0.1;Uid=root;Pwd=qwerty;Database=test;");


            // 2. Create database metadata
            var database = UniformDatabase.Create(config => config
                .RegisterDocument<UserDocument>()
                .RegisterDocument<QuestionDocument>()
                .RegisterDocument<CommentDocument>()
                .RegisterDocument<VoteDocument>()
                .RegisterDatabase(SampleDatabases.Mongodb, mongodbDatabase)
                .RegisterDatabase(SampleDatabases.Sql, mongodbDatabase)
            );

            database.EnterInMemoryMode();

            // 3. Optional.
            RunViewModelRegeneration(database);

            var stopwatch = Stopwatch.StartNew();
            database.LeaveInMemoryMode(true);
            stopwatch.Stop();
            Console.WriteLine("Flushed in {0:n0} ms", stopwatch.ElapsedMilliseconds);
        }

        public static void RunViewModelRegeneration(UniformDatabase database)
        {
            Console.Write("Creating list of events in memory... ");

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

//            SaveEventsToFile(events);
//            var result = LoadEventsFromFile();

            Console.WriteLine("Done. Managed memory used: {0:n0} mb.", GC.GetTotalMemory(false) / 1024 / 1024);

            var container = new UnityContainer();
            container.RegisterInstance<UniformDatabase>(database);

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
                    catch (EndOfStreamException)
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

        public static void FillDb(Int32 count)
        {
            List<AverageDocument> averages = new List<AverageDocument>();
            List<AnotherDocument> anothers = new List<AnotherDocument>();
            for (int i = 0; i < count; i++)
            {
                var average = new AverageDocument()
                {
                    Id = "average/" + i,
                    Name = "asdfasdf asdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsfasdfasd fadsf adsfadsf ",
                    Year = 4545,
                    Additional = "asdfasdfasdfasdf asdf adfasdf asd f",
                    AnotherId = "another/" + 5
                };
                averages.Add(average);
            }

            var another = new AnotherDocument()
            {
                Id = "another/" + 5,
                Name = "asdfasdf asdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsfasdfasd fadsf adsfadsf ",
                Year = 4545,
                Additional = "asdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdfasdfasdfasdfasdf asdf adfasdf asd f"
            };
            anothers.Add(another);

            var mdatabase = new MongodbDatabase("mongodb://localhost:27017/local");
            mdatabase.Database.GetCollection("averages").Drop();
            mdatabase.Database.GetCollection("anothers").Drop();
            mdatabase.Database.GetCollection("averages").InsertBatch(averages);
            mdatabase.Database.GetCollection("anothers").InsertBatch(anothers);

            var dbFactory = new OrmLiteConnectionFactory("server=127.0.0.1;Uid=root;Pwd=qwerty;Database=test;", MySqlDialectProvider.Instance);
            var connection = dbFactory.OpenDbConnection();
            var command = connection.CreateCommand();
            command.DropTable<AnotherDocument>();
            command.DropTable<AverageDocument>();
            command.CreateTable<AnotherDocument>();
            command.CreateTable<AverageDocument>();

            var transaction = connection.BeginTransaction();
            command.InsertAll(averages);
            command.InsertAll(anothers);
            transaction.Commit();
        }

        public static List<AllTogether> LoadAllTogether(IDbConnectionFactory factory)
        {
            using(var connection = factory.OpenDbConnection())
            {
                var cmd = connection.CreateCommand();
                var result = cmd.Select<AllTogether>("SELECT a.Id as AID, a.Name as AName, a.Hello as AHello, a.Year as AYear, " +
                    "a.Comments as AComments, a.Additional as AAdditional, a.AnotherId as AAnotherId, " +
                    "b.Id as BId, b.Name as BName, b.Hello as BHello, b.Year as BYear, " +
                    " b.Comments as BComments, b.Additional as BAdditional FROM test.averages a, test.anothers b where a.AnotherId = b.Id limit 50 ");

                return result;
            }
        }

        public static List<Selected> LoadSelected(IDbConnectionFactory factory)
        {
            using(var connection = factory.OpenDbConnection())
            {
                var cmd = connection.CreateCommand();
                var result = cmd.Select<Selected>("SELECT a.Id as AverageId, b.Id as AnotherId FROM test.averages a, test.anothers b where a.AnotherId = b.Id limit 50");
                return result;                
            }
        }

        public static BulkLoad SelectedToBulkLoad(List<Selected> selected)
        {
            var bulk = new BulkLoad();
            var av = bulk.Collections["averages"] = new BulkLoadCollection(typeof(AverageDocument));
            var an = bulk.Collections["anothers"] = new BulkLoadCollection(typeof(AnotherDocument));

            foreach (var one in selected)
            {
                av.Documents.TryAdd(one.AverageId, null);
                an.Documents.TryAdd(one.AnotherId, null);
            }

            return bulk;
        }

        public static void LoadTwoStep(IDbConnectionFactory factory, MongodbDatabase database)
        {
            var selected = LoadSelected(factory);
            var bulk = SelectedToBulkLoad(selected);
            var loader = new MongodbBulkLoader(database.Database, database.UniformDatabase.Metadata);
            loader.Load(bulk);
        }

        public static void TestLoading()
        {
/*
            var metadata = DatabaseMetadata.Create(config => config
                .AddDocumentType<AverageDocument>()
            );

*/
            var mdatabase = new MongodbDatabase("mongodb://localhost:27017/local");
            var dbFactory = new OrmLiteConnectionFactory("server=127.0.0.1;Uid=root;Pwd=qwerty;Database=test;", MySqlDialectProvider.Instance);
            var connection = dbFactory.OpenDbConnection();


            //FillDb(997);

            //var res = LoadAllTogether(connection);


            Console.ReadKey();
            var stopwatch = Stopwatch.StartNew();
            var count = 1000;
            var tasksCount = 10;

            var tasks = new Task[tasksCount];
            var half = count/tasksCount;
            for (int i = 0; i < half; i ++)
            {
                //LoadAllTogether(connection);
                //LoadSelected(connection);

                for (int j = 0; j < tasks.Length; j++)
                    tasks[j] = Task.Factory.StartNew(() => { LoadTwoStep(dbFactory, mdatabase); });
                    //tasks[j] = Task.Factory.StartNew(() => { LoadAllTogether(dbFactory); });

                Task.WaitAll(tasks);
            }
            stopwatch.Stop();
            Console.WriteLine("Loaded {0} times in {1}. Performing {2:0.00} requests/second", count,
                stopwatch.ElapsedMilliseconds, count / (stopwatch.ElapsedMilliseconds / (double) 1000));

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
