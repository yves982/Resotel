using System.Collections.Generic;
using System.Linq.Expressions;

namespace ResotelApp.Utils
{
    class SubstituteParametersVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> Substitutions
        {
            get;set;
        }

        public SubstituteParametersVisitor()
        {
            Substitutions = new Dictionary<Expression, Expression>();
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Expression result = node;
            if (Substitutions.ContainsKey(node))
            {
                result = Substitutions[node];
            }
            return result;
        }
    }
}
