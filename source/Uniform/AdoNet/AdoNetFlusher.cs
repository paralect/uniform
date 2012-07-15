using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;
using Uniform.InMemory;

namespace Uniform.AdoNet
{
    public class AdoNetFlusher
    {
        private readonly InMemoryDatabase _inMemoryDatabase;
        private readonly string _connectionString;
        private readonly OrmLiteConnectionFactory _dbFactory;
        private IDbConnection _connection;

        public AdoNetFlusher(InMemoryDatabase inMemoryDatabase, String connectionString)
        {
/*            _inMemoryDatabase = inMemoryDatabase;
            _connectionString = connectionString;

            _dbFactory = new OrmLiteConnectionFactory(connectionString, MySqlDialectProvider.Instance);
            _connection = _dbFactory.OpenDbConnection();*/
        }

        public void Flush<TDocument>() where TDocument : new()
        {
/*            var stopwatch = Stopwatch.StartNew();

            var cmd = _connection.CreateCommand();
            cmd.DropTable<TDocument>();
            cmd.CreateTable<TDocument>();

            var transaction = _connection.BeginTransaction();

            var collection = (InMemoryCollection<TDocument>) _inMemoryDatabase.GetCollection<TDocument>("test");

            
            cmd.InsertAll(collection.Documents.Values);
            transaction.Commit();


            stopwatch.Stop();

            Console.WriteLine("Done in {0:n0} ms", stopwatch.ElapsedMilliseconds);  */          
        }
    }
}