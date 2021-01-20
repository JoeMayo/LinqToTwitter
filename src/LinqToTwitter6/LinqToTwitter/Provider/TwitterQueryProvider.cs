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
 * Adopted and Modified By: Joe Mayo 8/26/08
 * *********************************************************/
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using LinqToTwitter.Common;

namespace LinqToTwitter.Provider
{
    /// <summary>
    /// implementation of IQueryProvider, part of the LINQ Provider API
    /// </summary>
    public class TwitterQueryProvider : IQueryProvider
    {
        /// <summary>
        /// refers to TwitterContext that calling code instantiated
        /// </summary>
        public TwitterContext? Context { get; set; }

        /// <summary>
        /// Non-generic version, returns current query to 
        /// calling code as its constructing the query
        /// </summary>
        /// <param name="expression">Expression tree</param>
        /// <returns>IQueryable that can be executed</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            Type? elementType = TypeSystem.GetElementType(expression.Type);

            _ = elementType ?? throw new TwitterQueryException("Expression doesn't have an element type.");

            try
            {
                object? twitterQueryableInstance = Activator.CreateInstance(
                    typeof(TwitterQueryable<>)
                        .MakeGenericType(elementType), 
                    new object[] { this, expression });

                _ = twitterQueryableInstance ?? throw new TwitterQueryException("Unable to create query instance.");

                return (IQueryable)twitterQueryableInstance;
            }
            catch (TargetInvocationException tie)
            {
                _ = tie?.InnerException ?? throw new InvalidOperationException("Invalid null exception");

                throw tie.InnerException;
            }
        }

        /// <summary>
        /// generic version, returns current query to 
        /// calling code as its constructing the query
        /// </summary>
        /// <typeparam name="TResult">current object type being worked with</typeparam>
        /// <param name="expression">expression tree for query</param>
        /// <returns>IQueryable that can be executed</returns>
        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new TwitterQueryable<TResult>(this, expression);
        }

        /// <summary>
        /// non-generic execute, delegates execution to TwitterContext
        /// </summary>
        /// <param name="expression">Expression Tree</param>
        /// <returns>list of results from query</returns>
        public object? Execute(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            return GetType().GetTypeInfo()
                .DeclaredMethods.Where(meth => meth.IsGenericMethod && meth.Name == "Execute").First()
                .Invoke(this, new object[] { expression });
        }

        /// <summary>
        /// generic execute, delegates execution to TwitterContext
        /// </summary>
        /// <typeparam name="TResult">type of query</typeparam>
        /// <param name="expression">Expression tree</param>
        /// <returns>list of results from query</returns>
        public TResult Execute<TResult>(Expression expression)
        {
            bool isEnumerable = 
                typeof(TResult).Name == "IEnumerable`1" ||
                typeof(TResult).Name == "IEnumerable";

            Type resultType = new MethodCallExpressionTypeFinder().GetGenericType(expression);
            var genericArguments = new[] { resultType };

            var methodInfo = Context?.GetType().GetTypeInfo().GetDeclaredMethod("ExecuteAsync");
            MethodInfo? genericMethodInfo = methodInfo?.MakeGenericMethod(genericArguments);

            try
            {
                var exeTask = Task.Run(() => (Task<object>)genericMethodInfo.Invoke(Context, new object[] { expression, isEnumerable }));
                return (TResult)exeTask.Result;
            }
            catch (TargetInvocationException tex)
            {
                // gotta unwrap the Invoke exception, as the the inner exception is the interesting bit...
                if (tex.InnerException != null)
                    throw tex.InnerException;
                throw;
            }
        }

        public async Task<object> ExecuteAsync<TResult>(Expression expression)
            where TResult : class
        {
            bool isEnumerable =
                typeof(TResult).Name == "IEnumerable`1" ||
                typeof(TResult).Name == "IEnumerable";

            Type resultType = new MethodCallExpressionTypeFinder().GetGenericType(expression);
            var genericArguments = new[] { resultType };

            var methodInfo = Context.GetType().GetTypeInfo().GetDeclaredMethod("ExecuteAsync");
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);

            try
            {
                var result = await ((Task<object>)genericMethodInfo.Invoke(Context, new object[] { expression, isEnumerable })).ConfigureAwait(false);
                return result;
            }
            catch (TargetInvocationException tex)
            {
                // gotta unwrap the Invoke exception, as the the inner exception is the interesting bit...
                if (tex.InnerException != null)
                    throw tex.InnerException;
                throw;
            }
        }
    }
}