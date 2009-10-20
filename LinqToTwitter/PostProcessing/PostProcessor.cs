using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace LinqToTwitter.PostProcessing
{
    internal class PostProcessor
    {
        /// <summary>
        /// Main entry point for post-processing queries as LINQ to Objects
        /// </summary>
        /// <typeparam name="T">Type of object being processed</typeparam>
        /// <param name="list">List of objects to process</param>
        /// <param name="expr">Expression Tree with Lambda to process</param>
        /// <returns>List of processed items</returns>
        internal IEnumerable ExecuteEnumerable<T>(IEnumerable<T> list, MethodCallExpression expr, bool isEnumerable)
        {
            IEnumerable<T> nextResult = null;

            if (expr.Arguments[0].NodeType == ExpressionType.Call)
            {
                nextResult = (IEnumerable<T>)ExecuteEnumerable<T>(list, expr.Arguments[0] as MethodCallExpression, isEnumerable);
            }
            else
            {
                nextResult = list;
            }

            IEnumerable operatorResult = null;

            var methodName = expr.Method.Name;
            var lambdaExpr = ((expr as MethodCallExpression).Arguments[1] as UnaryExpression).Operand as LambdaExpression;

            switch (methodName)
            {
                case "OrderBy":
                case "OrderByDescending":
                case "ThenBy":
                case "ThenByDescending":
                    operatorResult = new Sort().ProcessOrderBy<T>(methodName, nextResult, lambdaExpr);
                    break;
                case "Select":
                    operatorResult = new Projection().ProcessSelect<T>(nextResult, lambdaExpr);
                    break;
                case "Where":
                    operatorResult = list.Where((Func<T, bool>)lambdaExpr.Compile());
                    break;
                default:
                    break;
            }

            return operatorResult;
        }
    }
}
