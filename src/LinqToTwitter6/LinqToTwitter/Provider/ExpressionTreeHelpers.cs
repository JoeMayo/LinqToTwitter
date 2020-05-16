#nullable disable
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
using LinqToTwitter.Common;
using System;
using System.Linq.Expressions;

namespace LinqToTwitter.Provider
{
    /// <summary>
    /// utilities for working with expression trees
    /// </summary>
    internal class ExpressionTreeHelpers
    {
        /// <summary>
        /// test to see if expression is a binary expression that checks equality with a constant value
        /// - essentially, the caller wants to know if this is a well-formed expression with certain criteria
        /// </summary>
        /// <param name="exp">expression to check</param>
        /// <param name="declaringType">type containing member</param>
        /// <param name="memberName">member being checked</param>
        /// <returns>true if member is being checked for equality with value</returns>
        internal static bool IsMemberEqualsValueExpression(Expression exp, Type declaringType, string memberName)
        {
            if (exp.NodeType != ExpressionType.Equal &&
                exp.NodeType != ExpressionType.NotEqual &&
                exp.NodeType != ExpressionType.GreaterThan &&
                exp.NodeType != ExpressionType.GreaterThanOrEqual &&
                exp.NodeType != ExpressionType.LessThan &&
                exp.NodeType != ExpressionType.LessThanOrEqual)
                return false;

            BinaryExpression be = (BinaryExpression)exp;

            // Assert.
            if (IsSpecificMemberExpression(be.Left, declaringType, memberName) &&
                IsSpecificMemberExpression(be.Right, declaringType, memberName))
                throw new Exception("Cannot have 'member' == 'member' in an expression!");

            return (IsSpecificMemberExpression(be.Left, declaringType, memberName) ||
                IsSpecificMemberExpression(be.Right, declaringType, memberName));
        }

        /// <summary>
        /// verify that the type and member name in the expression are what is expected
        /// </summary>
        /// <param name="exp">expression to check</param>
        /// <param name="declaringType">expected type</param>
        /// <param name="memberName">expected member name</param>
        /// <returns>true if type and name in expression match expected type and name</returns>
        internal static bool IsSpecificMemberExpression(Expression exp, Type declaringType, string memberName)
        {
            // adjust for enums or VB ConvertChecked
            // VB wraps Type in a ConvertChecked that we must extract
            Expression tempExp =
                exp.NodeType == ExpressionType.Convert ||
                exp.NodeType == ExpressionType.ConvertChecked ?
                    (exp as UnaryExpression).Operand :
                    exp;

            return ((tempExp is MemberExpression expression) &&
                (expression.Member.DeclaringType == declaringType) &&
                (expression.Member.Name == memberName));
        }

        /// <summary>
        /// extracts the constant value from a binary equals expression
        /// - either the left or right side of the expression
        /// </summary>
        /// <param name="be">binary expression</param>
        /// <param name="memberDeclaringType">type of object</param>
        /// <param name="memberName">member to get value for</param>
        /// <returns>string representation of value</returns>
        internal static string GetValueFromEqualsExpression(BinaryExpression be, Type memberDeclaringType, string memberName)
        {
            if (be.NodeType != ExpressionType.Equal &&
                be.NodeType != ExpressionType.NotEqual &&
                be.NodeType != ExpressionType.GreaterThan &&
                be.NodeType != ExpressionType.GreaterThanOrEqual &&
                be.NodeType != ExpressionType.LessThan &&
                be.NodeType != ExpressionType.LessThanOrEqual)
                throw new Exception("There is a bug in this program.");

            if (be.Left.NodeType == ExpressionType.MemberAccess ||
                be.Left.NodeType == ExpressionType.Convert ||
                be.Left.NodeType == ExpressionType.ConvertChecked)
            {
                // adjust for enums & VB ConvertChecked
                MemberExpression me =
                    be.Left.NodeType == ExpressionType.Convert ||
                    be.Left.NodeType == ExpressionType.ConvertChecked ?
                        (be.Left as UnaryExpression).Operand as MemberExpression :
                        be.Left as MemberExpression;

                if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
                    return GetValueFromExpression(be.Right);
            }
            else if (be.Right.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)be.Right;

                if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
                    return GetValueFromExpression(be.Left);
            }

            // We should have returned by now.
            throw new Exception("There is a bug in this program.");
        }

        /// <summary>
        /// converts constant expression to constant value
        /// </summary>
        /// <param name="expression">constant expression</param>
        /// <returns>constant value</returns>
        internal static string GetValueFromExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)expression).Value.ToString();
            else if (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
                return ((int)((expression as UnaryExpression).Operand as ConstantExpression).Value).ToString();
            else
                throw new InvalidQueryException(
                    string.Format("The expression type {0} is not supported to obtain a value.", expression.NodeType));
        }
    }
}
