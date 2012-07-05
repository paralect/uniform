using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using C1.LiveLinq;
using C1.LiveLinq.Indexing;
using C1.LiveLinq.Collections;
using Remotion.Linq.Parsing.Structure;

namespace LivePlay
{
    public interface ITranslator<T> : IEnumerable<T>
    {
        IIndexedSource<T> Source { get; set; }
    }

    public static class TranslatorExtensions
    {
        public static ITranslator<T> Where<T>(this ITranslator<T> source, Expression<Func<T, bool>> predicate)
        {
            return new MyIndexedCollection<T>(source.Source.Where(predicate));
        }
    }

    public class MyIndexedCollection<T> : ITranslator<T>
    {
        public IIndexedSource<T> Source { get; set; }

        public MyIndexedCollection(IIndexedSource<T> source)
        {
            Source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Address : IndexableObject
    {
        private String _street;

        public string Street { get { return _street; } set { _street = value; OnPropertyChanged("Street"); } }
        public Int32 Number { get; set; }

        public Address(string street, int number)
        {
            _street = street;
            Number = number;
        }
    }

    public class Customer : IndexableObject
    {
        private String _name;
        private Int32 _year;

        public string Name { get { return _name; } set { _name = value; OnPropertyChanged("Name"); } }
        public Int32 Year { get { return _year; } set { _year = value; OnPropertyChanged("Year"); } }
        public string City { get; set; }

        public Address Address { get; set; }
//        public string Name { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var customers = new IndexedCollection<Customer>();
            customers.Indexes.Add(c => c.Name);
            customers.Indexes.Add(c => c.Year);
            customers.Indexes.Add(c => c.Address.Street);
            customers.Indexes.Add(c => c.City);

            for (int i = 0; i < 10000; i++)
            {
                customers.Add(new Customer { Name = "Hello" + i, Year = i, City = "Minsk", Address = new Address("Picadili" + i, i) });
                customers.Add(new Customer { Name = "Bye" + i, Year = i, City = "Lviv", Address = new Address("Dingo" + i, i) });
                customers.Add(new Customer { Name = "Greetings" + i, Year = i, City = "Moskow", Address = new Address("Bocasa" + i, i) });
            }

            var custurica = new Customer() { Name = "Custurica", Year = 34, City = "Hello", Address = new Address("Bogdana", 777) };
            customers.Add(custurica);

            var queryable = new MyIndexedCollection<Customer>(customers);


            Stopwatch watch = new Stopwatch();

//            var query2 = from c in customers where c.Name == "Hello" select c;
//            var list2 = query2.ToList();

            watch.Start();
            for (int i = 0; i < 1000; i++)
            {
                //var query = queryable.Where(c => c.Name == "Bye34"); //from c in customers where c.Name == "Bye34" select c;
                var query = queryable.Where(c => c.Year == 34 && c.Address.Street == "Bogdana" && c.Name == "Custurica" /*"Bye15"*/); //from c in customers where c.Name == "Bye34" select c;
                var list = query.ToList();
                Console.Write(list.Count);
            }
            Console.WriteLine();
            watch.Stop(); Console.Write("Search ended after {0} ms", watch.ElapsedMilliseconds);

            custurica.Name = "Bond";

            watch.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var query = from c in customers where c.Address.Street == "Bogdana" select c;
                var list = query.ToList();
                Console.Write(list.Count);
            }
            Console.WriteLine();
            watch.Stop(); Console.Write("Search ended after {0} ms", watch.ElapsedMilliseconds);


            Console.ReadKey();

        }
    }
}
