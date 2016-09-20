using System;
using System.Linq.Expressions;

namespace ResotelApp.Utils
{
    /// <summary>
    /// Allows to combine with "and" and "or" Linq Expressions for use within models
    /// </summary>
    static class PredicateExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T,bool>> left, Expression<Func<T,bool>> right)
            where T: class
        {
            SubstituteParametersVisitor substituteVisitor = new SubstituteParametersVisitor();
            substituteVisitor.Substitutions.Add(right.Parameters[0], left.Parameters[0]);
            Expression replacedRight = substituteVisitor.Visit(right.Body);
            BinaryExpression binaryExp = Expression.MakeBinary(ExpressionType.AndAlso, left.Body, replacedRight);
            return Expression.Lambda<Func<T, bool>>(binaryExp, left.Parameters[0]);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T,bool>> left, Expression<Func<T,bool>> right)
            where T : class
        {
            SubstituteParametersVisitor substituteVisitor = new SubstituteParametersVisitor();
            substituteVisitor.Substitutions.Add(right.Parameters[0], left.Parameters[0]);
            Expression replacedRight = substituteVisitor.Visit(right.Body);
            BinaryExpression binaryExp = Expression.MakeBinary(ExpressionType.OrElse, left.Body, replacedRight);
            return Expression.Lambda<Func<T, bool>>(binaryExp, left.Parameters[0]);
        }


        public static Expression<Func<T1, T2, bool>> And<T1,T2>(this Expression<Func<T1,T2, bool>> left, Expression<Func<T1,T2, bool>> right)
            where T1 : class
            where T2 : class
        {
            SubstituteParametersVisitor substituteVisitor = new SubstituteParametersVisitor();
            substituteVisitor.Substitutions.Add(right.Parameters[0], left.Parameters[0]);
            Expression replacedRight = substituteVisitor.Visit(right.Body);
            BinaryExpression binaryExp = Expression.MakeBinary(ExpressionType.AndAlso, left.Body, replacedRight);
            return Expression.Lambda<Func<T1, T2, bool>>(binaryExp, left.Parameters[0]);
        }

        public static Expression<Func<T1, T2, bool>> Or<T1,T2>(this Expression<Func<T1,T2, bool>> left, Expression<Func<T1, T2, bool>> right)
            where T1 : class
            where T2 : class
        {
            SubstituteParametersVisitor substituteVisitor = new SubstituteParametersVisitor();
            substituteVisitor.Substitutions.Add(right.Parameters[0], left.Parameters[0]);
            Expression replacedRight = substituteVisitor.Visit(right.Body);
            BinaryExpression binaryExp = Expression.MakeBinary(ExpressionType.OrElse, left.Body, replacedRight);
            return Expression.Lambda<Func<T1, T2, bool>>(binaryExp, left.Parameters[0]);
        }
    }
}
