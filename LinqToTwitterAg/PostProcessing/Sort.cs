using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter.PostProcessing
{
    internal class Sort
    {
        private delegate IOrderedEnumerable<T> SortOperation<T, TKey>(Func<T, TKey> compiledLambda);

        /// <summary>
        /// Select sort operation and execute
        /// </summary>
        /// <typeparam name="T">Type of object being sorted</typeparam>
        /// <typeparam name="TKey">Type of result from lambda</typeparam>
        /// <param name="sortType">Type of sort to perform</param>
        /// <param name="list">List to sort</param>
        /// <param name="lambdaExpr">Lambda expression for sort condition</param>
        /// <returns>Sorted List</returns>
        private IOrderedEnumerable<T> PerformSort<T, TKey>(string sortType, IEnumerable<T> list, LambdaExpression lambdaExpr)
        {
            SortOperation<T, TKey> sortOp = null;

            switch (sortType)
            {
                case "OrderBy":
                    sortOp = new SortOperation<T, TKey>(list.OrderBy);
                    break;
                case "OrderByDescending":
                    sortOp = new SortOperation<T, TKey>(list.OrderByDescending);
                    break;
                case "ThenBy":
                    sortOp = new SortOperation<T, TKey>((list as IOrderedEnumerable<T>).ThenBy);
                    break;
                case "ThenByDescending":
                    sortOp = new SortOperation<T, TKey>((list as IOrderedEnumerable<T>).ThenByDescending);
                    break;
                default:
                    break;
            }

            return sortOp((Func<T, TKey>)lambdaExpr.Compile());
        }

        /// <summary>
        /// Execute an OrderBy operation on the list
        /// </summary>
        /// <typeparam name="T">Type of elements in list</typeparam>
        /// <param name="sortType">Type of sort operation. i.e. OrderBy, OrderByDescending, ThenBy, or ThenByDescending</param>
        /// <param name="list">List of elements</param>
        /// <param name="lambdaExpr">Selector Lambda</param>
        /// <returns>List of ordered items</returns>
        internal IOrderedEnumerable<T> ProcessOrderBy<T>(string sortType, IEnumerable<T> list, LambdaExpression lambdaExpr)
        {
            IOrderedEnumerable<T> operatorResult = null;

            switch (lambdaExpr.Body.Type.Name)
            {
                case "Boolean":
                    operatorResult = PerformSort<T, bool>(sortType, list, lambdaExpr);
                    break;
                case "DateTime":
                    operatorResult = PerformSort<T, DateTime>(sortType, list, lambdaExpr);
                    break;
                case "Decimal":
                    operatorResult = PerformSort<T, decimal>(sortType, list, lambdaExpr);
                    break;
                case "Double":
                    operatorResult = PerformSort<T, double>(sortType, list, lambdaExpr);
                    break;
                case "Int32":
                    operatorResult = PerformSort<T, int>(sortType, list, lambdaExpr);
                    break;
                case "Single":
                    operatorResult = PerformSort<T, float>(sortType, list, lambdaExpr);
                    break;
                case "String":
                    operatorResult = PerformSort<T, string>(sortType, list, lambdaExpr);
                    break;
                case "UInt64":
                    operatorResult = PerformSort<T, ulong>(sortType, list, lambdaExpr);
                    break;
                default:
                    throw new ArgumentException(
                        "Type " + lambdaExpr.Body.Type.Name + " is not yet supported. Please discuss or submit an issue at http://linqtotwitter.codeplex.com/");
            }

            return operatorResult;
        }

        ///// <summary>
        ///// Execute an OrderBy operation on the list
        ///// </summary>
        ///// <typeparam name="T">Type of elements in list</typeparam>
        ///// <param name="list">List of elements</param>
        ///// <param name="lambdaExpr">Selector Lambda</param>
        ///// <returns>List of ordered items</returns>
        //internal IOrderedEnumerable<T> ProcessOrderBy<T>(IEnumerable<T> list, LambdaExpression lambdaExpr)
        //{
        //    IOrderedEnumerable<T> operatorResult = null;

        //    switch (lambdaExpr.Body.Type.Name)
        //    {
        //        case "Boolean":
        //            operatorResult = list.OrderBy((Func<T, bool>)lambdaExpr.Compile());
        //            break;
        //        case "DateTime":
        //            operatorResult = list.OrderBy((Func<T, DateTime>)lambdaExpr.Compile());
        //            break;
        //        case "Decimal":
        //            operatorResult = list.OrderBy((Func<T, decimal>)lambdaExpr.Compile());
        //            break;
        //        case "Double":
        //            operatorResult = list.OrderBy((Func<T, double>)lambdaExpr.Compile());
        //            break;
        //        case "Int32":
        //            operatorResult = list.OrderBy((Func<T, int>)lambdaExpr.Compile());
        //            break;
        //        case "Single":
        //            operatorResult = list.OrderBy((Func<T, float>)lambdaExpr.Compile());
        //            break;
        //        case "String":
        //            operatorResult = list.OrderBy((Func<T, string>)lambdaExpr.Compile());
        //            break;
        //        case "UInt64":
        //            operatorResult = list.OrderBy((Func<T, ulong>)lambdaExpr.Compile());
        //            break;
        //        default:
        //            throw new ArgumentException(
        //                "Type " + lambdaExpr.Body.Type.Name + " is not yet supported. Please discuss or submit an issue at http://linqtotwitter.codeplex.com/");
        //    }

        //    return operatorResult;
        //}

        ///// <summary>
        ///// Execute an OrderByDecending operation on the list
        ///// </summary>
        ///// <typeparam name="T">Type of elements in list</typeparam>
        ///// <param name="list">List of elements</param>
        ///// <param name="lambdaExpr">Selector Lambda</param>
        ///// <returns>List of ordered items</returns>
        //internal IEnumerable<T> ProcessOrderByDescending<T>(IEnumerable<T> list, LambdaExpression lambdaExpr)
        //{
        //    IEnumerable<T> operatorResult = null;

        //    switch (lambdaExpr.Body.Type.Name)
        //    {
        //        case "Boolean":
        //            operatorResult = list.OrderByDescending((Func<T, bool>)lambdaExpr.Compile());
        //            break;
        //        case "DateTime":
        //            operatorResult = list.OrderByDescending((Func<T, DateTime>)lambdaExpr.Compile());
        //            break;
        //        case "Decimal":
        //            operatorResult = list.OrderByDescending((Func<T, decimal>)lambdaExpr.Compile());
        //            break;
        //        case "Double":
        //            operatorResult = list.OrderByDescending((Func<T, double>)lambdaExpr.Compile());
        //            break;
        //        case "Int32":
        //            operatorResult = list.OrderByDescending((Func<T, int>)lambdaExpr.Compile());
        //            break;
        //        case "Single":
        //            operatorResult = list.OrderByDescending((Func<T, float>)lambdaExpr.Compile());
        //            break;
        //        case "String":
        //            operatorResult = list.OrderByDescending((Func<T, string>)lambdaExpr.Compile());
        //            break;
        //        case "UInt64":
        //            operatorResult = list.OrderByDescending((Func<T, ulong>)lambdaExpr.Compile());
        //            break;
        //        default:
        //            throw new ArgumentException(
        //                "Type " + lambdaExpr.Body.Type.Name + " is not yet supported. Please discuss or submit an issue at http://linqtotwitter.codeplex.com/");
        //    }

        //    return operatorResult;
        //}

        ///// <summary>
        ///// Execute an ThenBy operation on the list
        ///// </summary>
        ///// <typeparam name="T">Type of elements in list</typeparam>
        ///// <param name="list">List of elements</param>
        ///// <param name="lambdaExpr">Selector Lambda</param>
        ///// <returns>List of ordered items</returns>
        //internal IEnumerable<T> ProcessThenBy<T>(IOrderedEnumerable<T> list, LambdaExpression lambdaExpr)
        //{
        //    IOrderedEnumerable<T> operatorResult = null;

        //    switch (lambdaExpr.Body.Type.Name)
        //    {
        //        case "Boolean":
        //            operatorResult = list.ThenBy((Func<T, bool>)lambdaExpr.Compile());
        //            break;
        //        case "DateTime":
        //            operatorResult = list.ThenBy((Func<T, DateTime>)lambdaExpr.Compile());
        //            break;
        //        case "Decimal":
        //            operatorResult = list.ThenBy((Func<T, decimal>)lambdaExpr.Compile());
        //            break;
        //        case "Double":
        //            operatorResult = list.ThenBy((Func<T, double>)lambdaExpr.Compile());
        //            break;
        //        case "Int32":
        //            operatorResult = list.ThenBy((Func<T, int>)lambdaExpr.Compile());
        //            break;
        //        case "Single":
        //            operatorResult = list.ThenBy((Func<T, float>)lambdaExpr.Compile());
        //            break;
        //        case "String":
        //            operatorResult = list.ThenBy((Func<T, string>)lambdaExpr.Compile());
        //            break;
        //        case "UInt64":
        //            operatorResult = list.ThenBy((Func<T, ulong>)lambdaExpr.Compile());
        //            break;
        //        default:
        //            throw new ArgumentException(
        //                "Type " + lambdaExpr.Body.Type.Name + " is not yet supported. Please discuss or submit an issue at http://linqtotwitter.codeplex.com/");
        //    }

        //    return operatorResult;
        //}

        ///// <summary>
        ///// Execute an ThenByDecending operation on the list
        ///// </summary>
        ///// <typeparam name="T">Type of elements in list</typeparam>
        ///// <param name="list">List of elements</param>
        ///// <param name="lambdaExpr">Selector Lambda</param>
        ///// <returns>List of ordered items</returns>
        //internal IEnumerable<T> ProcessThenByDescending<T>(IOrderedEnumerable<T> list, LambdaExpression lambdaExpr)
        //{
        //    IOrderedEnumerable<T> operatorResult = null;

        //    switch (lambdaExpr.Body.Type.Name)
        //    {
        //        case "Boolean":
        //            operatorResult = list.ThenByDescending((Func<T, bool>)lambdaExpr.Compile());
        //            break;
        //        case "DateTime":
        //            operatorResult = list.ThenByDescending((Func<T, DateTime>)lambdaExpr.Compile());
        //            break;
        //        case "Decimal":
        //            operatorResult = list.ThenByDescending((Func<T, decimal>)lambdaExpr.Compile());
        //            break;
        //        case "Double":
        //            operatorResult = list.ThenByDescending((Func<T, double>)lambdaExpr.Compile());
        //            break;
        //        case "Int32":
        //            operatorResult = list.ThenByDescending((Func<T, int>)lambdaExpr.Compile());
        //            break;
        //        case "Single":
        //            operatorResult = list.ThenByDescending((Func<T, float>)lambdaExpr.Compile());
        //            break;
        //        case "String":
        //            operatorResult = list.ThenByDescending((Func<T, string>)lambdaExpr.Compile());
        //            break;
        //        case "UInt64":
        //            operatorResult = list.ThenByDescending((Func<T, ulong>)lambdaExpr.Compile());
        //            break;
        //        default:
        //            throw new ArgumentException(
        //                "Type " + lambdaExpr.Body.Type.Name + " is not yet supported. Please discuss or submit an issue at http://linqtotwitter.codeplex.com/");
        //    }

        //    return operatorResult;
        //}
    }
}
