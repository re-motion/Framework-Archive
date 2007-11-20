using System.Collections.Generic;
using System.Linq.Expressions;
using Rubicon.Text;

namespace Rubicon.Data.DomainObjects.Linq.Parsing
{
  public static class ParserUtility
  {
    public static T GetTypedExpression<T> (object expression, string context, Expression expressionTreeRoot)
    {
      if (!(expression is T))
      {
        string message = string.Format ("Expected {0} for {1}, found {2} ({3}).", typeof (T).Name, context, expression.GetType().Name, expression);
        throw new QueryParserException (message, expression, expressionTreeRoot);
      }
      else
        return (T) expression;
    }

    public static string CheckMethodCallExpression (MethodCallExpression methodCallExpression, Expression expressionTreeRoot,
        params string[] expectedMethodNames)
    {
      if (!((IList<string>)expectedMethodNames).Contains (methodCallExpression.Method.Name))
      {
        string message = string.Format ("Expected one of '{0}', but found '{1}' at position {2} in tree {3}.",
            SeparatedStringBuilder.Build (", ", expectedMethodNames),methodCallExpression.Method.Name, methodCallExpression, expressionTreeRoot);
        throw new QueryParserException (message, methodCallExpression, expressionTreeRoot);
      }
      else
        return methodCallExpression.Method.Name;
    }
  }
}