using System;
using System.Linq;
using IndexedLinq.IndexedProvider;
using NUnit.Framework;
using Remotion.Linq.Parsing.Structure;

namespace IndexedLinq.Tests
{
    [TestFixture]
    public class RelinqSampleTests
    {
        private IndexedProviderQueryable<SampleDataSourceItem> items;

        [SetUp]
        public void SetUp()
        {
            var queryParser = QueryParser.CreateDefault();
            items = new IndexedProviderQueryable<SampleDataSourceItem>(queryParser, new IndexedProviderQueryExecutor());
        }

        [Test]
        public void Test()
        {
            var nanan = "Name 1";

            var results = (from i in items 
                          where i.Name == "Name 3" || i.Name == "Name 2" || i.Name == nanan
                          select i).Where(i => i.Name == "Name 7");

            var list = results.ToList();

            Assert.That(list.Count, Is.EqualTo(10));
            Assert.That(list[3].Name, Is.EqualTo("Name 3"));
        }
    }
}