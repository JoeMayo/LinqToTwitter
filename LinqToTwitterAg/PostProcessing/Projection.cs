using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;

namespace LinqToTwitter.PostProcessing
{
    internal class Projection
    {
        /// <summary>
        /// Execute a projection on the list
        /// </summary>
        /// <typeparam name="T">Type being processed</typeparam>
        /// <param name="list">List to process</param>
        /// <param name="lambdaExpr">Lambda with projection</param>
        /// <returns>List of projected types</returns>
        internal IEnumerable ProcessSelect<T>(IEnumerable<T> list, LambdaExpression lambdaExpr)
        {
            Type resultType = TypeSystem.GetElementType(lambdaExpr.Body.Type);

            var methodInfo =
                typeof(Enumerable)
                    .GetMethods()
                    .Where(
                        meth => meth.Name == "Select" &&
                        meth.GetGenericArguments().Count() == 2)
                    .FirstOrDefault();

            Type[] genericArguments = new Type[] { typeof(T), resultType };
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);

            return (IEnumerable)genericMethodInfo.Invoke(null, new object[] { list, lambdaExpr.Compile() });
        }
    }
}
