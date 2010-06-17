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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace LinqToTwitter
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
        private Expression m_expression;

        /// <summary>
        /// parameters to search for
        /// </summary>
        private Dictionary<string, string> m_parameters;

        /// <summary>
        /// keep track of expression and parameter list
        /// </summary>
        /// <param name="exp">expression to search</param>
        /// <param name="parameters">parameters to search for</param>
        public ParameterFinder(Expression exp, List<string> parameters)
        {
            m_expression = exp;
            ParameterNames = parameters;
        }

        /// <summary>
        /// name/value pairs of parameters and their values
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get
            {
                if (m_parameters == null)
                {
                    m_parameters = new Dictionary<string, string>();
                    Visit(m_expression);
                }
                return m_parameters;
            }
        }

        /// <summary>
        /// names of input parameters
        /// </summary>
        public List<string> ParameterNames { get; set; }

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
                foreach (var param in ParameterNames)
                {
                    if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(T), param))
                    {
                        m_parameters.Add(param, ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(T), param));
                        return be;
                    }
                }

                return base.VisitBinary(be);
            }
            else
                return base.VisitBinary(be);
        }
    }
}
