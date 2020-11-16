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
using System.Linq.Expressions;
#if !ASYNC
using LinqToTwitter.Common;
#endif

namespace LinqToTwitter.Provider
{
    /// <summary>
    /// translates variable references in expressions to constants
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression? PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression? PartialEval(Expression expression)
        {
            return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
        }

        static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        class SubtreeEvaluator : ExpressionVisitor
        {
            readonly HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression? Eval(Expression exp)
            {
                return Visit(exp);
            }

            public override Expression? Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (candidates.Contains(exp))
                {
                    return Evaluate(exp);
                }
                return base.Visit(exp);
            }

            Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                LambdaExpression lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        class Nominator : ExpressionVisitor
        {
            readonly Func<Expression, bool> fnCanBeEvaluated;
            HashSet<Expression>? candidates;
            bool cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                candidates = new HashSet<Expression>();
                Visit(expression);
                return candidates;
            }

            public override Expression? Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = cannotBeEvaluated;
                    cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!cannotBeEvaluated)
                    {
                        if (fnCanBeEvaluated(expression))
                        {
                            candidates?.Add(expression);
                        }
                        else
                        {
                            cannotBeEvaluated = true;
                        }
                    }
                    cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
        }
    }
}
