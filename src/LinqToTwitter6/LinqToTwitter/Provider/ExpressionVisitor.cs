/***********************************************************
 * Credits:
 * 
 * Matt Warren's Blog -
 * LINQ: Building an IQueryable Provider:
 * 
 * http://blogs.msdn.com/b/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
 * *********************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace LinqToTwitter.Provider
{
    public abstract class ExpressionVisitor
    {
        public virtual Expression? Visit(Expression? exp)
        {
            if (exp == null)
                return exp;

            return exp.NodeType switch
            {
                ExpressionType.Negate or ExpressionType.NegateChecked or ExpressionType.Not or ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.ArrayLength or ExpressionType.Quote or ExpressionType.TypeAs => this.VisitUnary((UnaryExpression)exp),
                ExpressionType.Add or ExpressionType.AddChecked or ExpressionType.Subtract or ExpressionType.SubtractChecked or ExpressionType.Multiply or ExpressionType.MultiplyChecked or ExpressionType.Divide or ExpressionType.Modulo or ExpressionType.And or ExpressionType.AndAlso or ExpressionType.Or or ExpressionType.OrElse or ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual or ExpressionType.Equal or ExpressionType.NotEqual or ExpressionType.Coalesce or ExpressionType.ArrayIndex or ExpressionType.RightShift or ExpressionType.LeftShift or ExpressionType.ExclusiveOr => this.VisitBinary((BinaryExpression)exp),
                ExpressionType.TypeIs => this.VisitTypeIs((TypeBinaryExpression)exp),
                ExpressionType.Conditional => this.VisitConditional((ConditionalExpression)exp),
                ExpressionType.Constant => this.VisitConstant((ConstantExpression)exp),
                ExpressionType.Parameter => this.VisitParameter((ParameterExpression)exp),
                ExpressionType.MemberAccess => this.VisitMemberAccess((MemberExpression)exp),
                ExpressionType.Call => this.VisitMethodCall((MethodCallExpression)exp),
                ExpressionType.Lambda => this.VisitLambda((LambdaExpression)exp),
                ExpressionType.New => this.VisitNew((NewExpression)exp),
                ExpressionType.NewArrayInit or ExpressionType.NewArrayBounds => this.VisitNewArray((NewArrayExpression)exp),
                ExpressionType.Invoke => this.VisitInvocation((InvocationExpression)exp),
                ExpressionType.MemberInit => this.VisitMemberInit((MemberInitExpression)exp),
                ExpressionType.ListInit => this.VisitListInit((ListInitExpression)exp),
                _ => throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType)),
            };
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            return binding.BindingType switch
            {
                MemberBindingType.Assignment => this.VisitMemberAssignment((MemberAssignment)binding),
                MemberBindingType.MemberBinding => this.VisitMemberMemberBinding((MemberMemberBinding)binding),
                MemberBindingType.ListBinding => this.VisitMemberListBinding((MemberListBinding)binding),
                _ => throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType)),
            };
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);

            if (arguments != initializer.Arguments)
                return Expression.ElementInit(initializer.AddMethod, arguments);

            return initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression? operand = this.Visit(u.Operand);

            if (operand != u.Operand)
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);

            return u;
        }

        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression? left = this.Visit(b.Left);
            Expression? right = this.Visit(b.Right);
            Expression? conversion = this.Visit(b.Conversion);

            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }

            return b;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression? expr = this.Visit(b.Expression);

            if (expr != b.Expression)
                return Expression.TypeIs(expr, b.TypeOperand);

            return b;
        }

        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression? test = this.Visit(c.Test);
            Expression? ifTrue = this.Visit(c.IfTrue);
            Expression? ifFalse = this.Visit(c.IfFalse);

            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
                return Expression.Condition(test, ifTrue, ifFalse);

            return c;
        }

        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression? exp = this.Visit(m.Expression);

            if (exp != m.Expression)
                return Expression.MakeMemberAccess(exp, m.Member);

            return m;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression? obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);

            if (obj != m.Object || args != m.Arguments)
                return Expression.Call(obj, m.Method, args);

            return m;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression? p = this.Visit(original[i]);

                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);

                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(p);
                }
            }

            if (list != null)
                return new ReadOnlyCollection<Expression>(list);

            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression? e = this.Visit(assignment.Expression);

            if (e != assignment.Expression)
                return Expression.Bind(assignment.Member, e);

            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);

            if (bindings != binding.Bindings)
                return Expression.MemberBind(binding.Member, bindings);

            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);

            if (initializers != binding.Initializers)
                return Expression.ListBind(binding.Member, initializers);

            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding>? list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);

                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);

                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(b);
                }
            }

            if (list != null)
                return list;

            return original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit>? list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);

                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);

                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }

                    list.Add(init);
                }
            }

            if (list != null)
                return list;

            return original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression? body = this.Visit(lambda.Body);

            if (body != lambda.Body)
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);

            return lambda;
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);

            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                    return Expression.New(nex.Constructor, args, nex.Members);
                else
                    return Expression.New(nex.Constructor, args);
            }

            return nex;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);

            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);

            if (n != init.NewExpression || bindings != init.Bindings)
                return Expression.MemberInit(n, bindings);

            return init;
        }

        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);

            if (n != init.NewExpression || initializers != init.Initializers)
                return Expression.ListInit(n, initializers);

            return init;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);

            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
                }
                else
                {
                    return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
                }
            }

            return na;
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression? expr = this.Visit(iv.Expression);

            if (args != iv.Arguments || expr != iv.Expression)
                return Expression.Invoke(expr, args);

            return iv;
        }
    }
}
