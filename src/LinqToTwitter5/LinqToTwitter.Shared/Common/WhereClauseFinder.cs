using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter.Common
{
    /// <summary>
    /// finds where clauses in the expression tree
    /// </summary>
    internal class WhereClauseFinder : ExpressionVisitor
    {
        static readonly string[] WhereMethodNames = { "Where", "Single", "SingleOrDefault", "First", "FirstOrDefault" };

        // holds all where expressions
        readonly List<MethodCallExpression> whereExpressions = new List<MethodCallExpression>();

        /// <summary>
        /// searches expression tree for wheres and returns collection of all it finds.
        /// </summary>
        /// <param name="expression">query expression to search.</param>
        /// <returns>collection of where expressions.</returns>
        public MethodCallExpression[] GetAllWheres(Expression expression)
        {
            Visit(expression);
            return whereExpressions.ToArray();
        }

        /// <summary>
        /// custom processing of MethodCallExpression NodeType that checks for a
        /// where clause and retains expression as member of list of where clauses.
        /// </summary>
        /// <param name="expression">a MethodCallExpression node from the expression tree</param>
        /// <returns>expression that was passed in</returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (WhereMethodNames.Contains(expression.Method.Name) && expression.Arguments.Count == 2)
            {
                whereExpressions.Add(expression);
            }

            Visit(expression.Arguments[0]);

            return expression;
        }
    }
}
