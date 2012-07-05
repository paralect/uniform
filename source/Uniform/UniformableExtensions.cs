using System;
using System.Linq.Expressions;

namespace Uniform
{
    public static class UniformableExtensions
    {
        public static IUniformable<T> Where<T>(this IUniformable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source.Provider.Where(source, predicate);
        }

        public static IUniformable<TResult> Select<TSource, TResult>(this IUniformable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return source.Provider.Select(source, selector);
        }
    }
}