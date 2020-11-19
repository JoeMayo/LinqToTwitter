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
 * *********************************************************/
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToTwitter.Provider
{
    /// <summary>
    /// extracts parameters from an expression
    /// - called for extracting parameters and values on where clauses
    /// </summary>
    /// <typeparam name="T">type to get parameters for</typeparam>
    internal class ParameterFinder<T> : ExpressionVisitor
    {
        /// <summary>
        /// expression being searched
        /// </summary>
        readonly Expression? expression;

        /// <summary>
        /// parameters to search for
        /// </summary>
        readonly Dictionary<string, string> parameters;

        /// <summary>
        /// names of input parameters
        /// </summary>
        readonly List<string> parameterNames;

        /// <summary>
        /// keep track of expression and parameter list
        /// </summary>
        /// <param name="expression">expression to search</param>
        /// <param name="parameterNames">parameters to search for</param>
        public ParameterFinder(Expression expression, List<string> parameterNames)
        {
            this.parameters = new Dictionary<string, string>();
            this.expression = expression;
            this.parameterNames = parameterNames;
        }

        /// <summary>
        /// name/value pairs of parameters and their values
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get
            {
                Visit(expression);
                return parameters;
            }
        }

        /// <summary>
        /// extracts values from equality expressions that match parameter names
        /// </summary>
        /// <param name="be">binary expression to evaluate</param>
        /// <returns>binary expression - supports recursive tree traversal in visitor</returns>
        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal || 
                be.NodeType == ExpressionType.GreaterThan ||
                be.NodeType == ExpressionType.GreaterThanOrEqual ||
                be.NodeType == ExpressionType.LessThan ||
                be.NodeType == ExpressionType.LessThanOrEqual ||
                be.NodeType == ExpressionType.NotEqual)
            {
                foreach (var param in parameterNames)
                {
                    if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(T), param))
                    {
                        parameters.Add(param, ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(T), param));
                        return be;
                    }
                }

                return base.VisitBinary(be);
            }
            else
                return base.VisitBinary(be);
        }

        protected override Expression VisitMethodCall(MethodCallExpression me)
        {

            foreach (var param in parameterNames)
            {
                if (me?.Method.Name == "CompareString" && (me?.Arguments[0] as MemberExpression)?.Member.Name == param)
                {
                    string? constantValue = (me.Arguments[1] as ConstantExpression)?.Value?.ToString();
                    
                    if (constantValue != null)
                        parameters.Add(param, constantValue);
                    
                    return me;
                } 
            }

            return base.VisitMethodCall(me);
        }
    }
}
