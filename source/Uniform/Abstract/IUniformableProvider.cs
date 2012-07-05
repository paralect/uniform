using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using C1.LiveLinq.Indexing;
using C1.LiveLinq;
using Uniform.InMemory;
using Uniform.Mongodb;
using System.Linq;

namespace Uniform
{
    public interface IUniformableProvider
    {
        IUniformable<T> Where<T>(IUniformable<T> source, Expression<Func<T, bool>> predicate);
        IUniformable<TResult> Select<TSource, TResult>(IUniformable<TSource> source, Expression<Func<TSource, TResult>> selector);
    }

    public class InMemoryUniformableProvider : IUniformableProvider
    {
        public IUniformable<T> Where<T>(IUniformable<T> source, Expression<Func<T, bool>> predicate)
        {
            var memorySource = (InMemorySource<T>) source;
            return new InMemorySource<T>(memorySource.Source.Where(predicate));
        }

        public IUniformable<TResult> Select<TSource, TResult>(IUniformable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var memorySource = (InMemorySource<TSource>) source;
            return new InMemorySource<TResult>(memorySource.Source.Select(selector));
        }
    }

    public class MongodbUniformableProvider : IUniformableProvider
    {
        public IUniformable<T> Where<T>(IUniformable<T> source, Expression<Func<T, bool>> predicate)
        {
            var mongodbSource = (MongodbSource<T>) source;
            return new MongodbSource<T>(mongodbSource.Source.Where(predicate));
        }

        public IUniformable<TResult> Select<TSource, TResult>(IUniformable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var mongodbSource = (MongodbSource<TSource>)source;
            return new MongodbSource<TResult>(mongodbSource.Source.Select(selector));
        }
    }
}