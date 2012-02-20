using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitter
{
    class ExpressionTreeModifier<T> : ExpressionVisitor
    {
        readonly IQueryable<T> queryableItems;

        internal ExpressionTreeModifier(IQueryable<T> items)
        {
            queryableItems = items;
        }

        internal Expression CopyAndModify(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Replace the constant TwitterQueryable arg with the queryable collection.
            if (c.Type.Name == "TwitterQueryable`1")
                return Expression.Constant(queryableItems);
            
            return c;
        }
    }
}
