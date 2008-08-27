/***********************************************************
 * Credits:
 * 
 * MSDN Documentation -
 * Walkthrough: Creating an IQueryable LINQ Provider
 * 
 * http://msdn.microsoft.com/en-us/library/bb546158.aspx
 * 
 * Matt Warren's Blog -
 * LINQ: Building an IQueryable Provider:
 * 
 * http://blogs.msdn.com/mattwar/default.aspx
 * 
 * Modified By: Joe Mayo 8/26/08
 * 
 * - Added Context property
 * - Changed Execute to delegate to TwitterContext through Context property
 * *********************************************************/

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToTwitter
{
    public class TwitterQueryProvider : IQueryProvider
    {
        public TwitterContext Context { get; set; }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(
                    typeof(TwitterQueryable<>)
                        .MakeGenericType(elementType), 
                    new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new TwitterQueryable<TResult>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Context.Execute(expression, false);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            bool IsEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            return (TResult)Context.Execute(expression, IsEnumerable);
        }
    }
}