using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    class ExpressionTreeModifier<T> : ExpressionVisitor
    {
        private IQueryable<T> queryableItems;

        internal ExpressionTreeModifier(IQueryable<T> items)
        {
            this.queryableItems = items;
        }

        internal Expression CopyAndModify(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Replace the constant TwitterQueryable arg with the queryable collection.
            if (c.Type.Name == "TwitterQueryable`1")
                return Expression.Constant(this.queryableItems);
            else
                return c;
        }
    }
}
