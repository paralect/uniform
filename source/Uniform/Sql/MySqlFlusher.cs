using System;
using System.Data;
using System.Diagnostics;
using System.Text;
using MySql.Data.MySqlClient;
using Uniform.InMemory;

namespace Uniform.Sql
{
    public class MySqlFlusher
    {
        private readonly InMemoryDatabase _inMemoryDatabase;
        private readonly MySqlConnection _connection;

        public MySqlFlusher(InMemoryDatabase inMemoryDatabase, String connectionString)
        {
            _inMemoryDatabase = inMemoryDatabase;
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        }

        public void Flush()
        {
            var stopwatch = Stopwatch.StartNew();

            var transaction = _connection.BeginTransaction();
            var stringBuilder = new StringBuilder();
            for (int index = 0; index < 100001; index++)
            {
                stringBuilder.AppendFormat(
                    "insert into users (UserId, UserName, About, Aga) values ('{0}', '{1}', '{2}', '{3}'); \n",
                    "user/" + index, "Final Tonny!", "Some words",
                    "Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях. Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях. Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях. Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях.");

                if (index % 1000 == 0 && index != 0)
                {
                    var command = _connection.CreateCommand();
                    command.CommandText = stringBuilder.ToString();
                    command.ExecuteNonQuery();
                    stringBuilder.Clear();
                }
            }

            transaction.Commit();
            

            stopwatch.Stop();

            Console.WriteLine("Done in {0:n0} ms", stopwatch.ElapsedMilliseconds);
        }
    }
}