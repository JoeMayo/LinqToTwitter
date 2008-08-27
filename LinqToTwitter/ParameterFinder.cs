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
    internal class ParameterFinder<T> : ExpressionVisitor
    {
        private Expression m_expression;
        private Dictionary<string, string> m_parameters;

        public ParameterFinder(Expression exp, List<string> parameters)
        {
            m_expression = exp;
            ParameterNames = parameters;
        }

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

        public List<string> ParameterNames { get; set; }

        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
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
