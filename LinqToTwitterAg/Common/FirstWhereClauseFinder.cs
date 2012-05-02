/***********************************************************
 * Credits:
 * 
 * MSDN Documentation -
 * Walkthrough: Creating an IQueryable LINQ Provider
 * 
 * http://msdn.microsoft.com/en-us/library/bb546158.aspx
 * 
 * Renamed and Documented By: Joe Mayo, 10/12/08
 * *********************************************************/

using System.Linq.Expressions;

namespace LinqToTwitter
{
    /// <summary>
    /// finds the first where clause in the expression tree
    /// </summary>
    internal class FirstWhereClauseFinder : ExpressionVisitor
    {
        /// <summary>
        /// holds first where expression when found
        /// </summary>
        private MethodCallExpression firstWhereExpression;

        /// <summary>
        /// initiates search for first where clause
        /// </summary>
        /// <param name="expression">expression tree to search</param>
        /// <returns>MethodCallExpression for first where clause</returns>
        public MethodCallExpression GetFirstWhere(Expression expression)
        {
            Visit(expression);
            return firstWhereExpression;
        }

        /// <summary>
        /// custom processing of MethodCallExpression NodeType that checks for a
        /// where clause and retains expression as first where if it is a where clause
        /// </summary>
        /// <param name="expression">a MethodCallExpression node from the expression tree</param>
        /// <returns>expression that was passed in</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Where")
                firstWhereExpression = expression;

            // look at extension source to see if there is an earlier where
            Visit(expression.Arguments[0]);

            return expression;
        }
    }
}
