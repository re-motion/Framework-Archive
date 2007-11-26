using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Linq.Parsing;
using Rubicon.Data.DomainObjects.Linq.UnitTests.Parsing;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Linq.UnitTests.ParsingTest.WhereExpressionParserTest
{
  [TestFixture]
  public class GeneralWhereExpressionParserTest
  {
    [Test]
    public void Initialize ()
    {
      MethodCallExpression expression = TestQueryGenerator.CreateSimpleWhereQuery_WhereExpression(ExpressionHelper.CreateQuerySource());
      WhereExpressionParser parser = new WhereExpressionParser (expression, expression, true);
      Assert.AreSame (expression, parser.SourceExpression);
    }

    [Test]
    [ExpectedException (typeof (QueryParserException), ExpectedMessage = "Expected one of 'Where', but found 'Select' at position "
                                                                         + "value(Rubicon.Data.DomainObjects.Linq.QueryProviderImplementation.StandardQueryable`1[Rubicon.Data.DomainObjects.Linq.UnitTests.Parsing."
                                                                         + "Student]).Select(s => s) in tree value(Rubicon.Data.DomainObjects.Linq.QueryProviderImplementation.StandardQueryable`1[Rubicon.Data."
                                                                         + "DomainObjects.Linq.UnitTests.Parsing.Student]).Select(s => s).")]
    public void Initialize_FromWrongExpression ()
    {
      MethodCallExpression expression = TestQueryGenerator.CreateSimpleQuery_SelectExpression (ExpressionHelper.CreateQuerySource ());
      new WhereExpressionParser (expression, expression, true);
    }

    [Test]
    [ExpectedException (typeof (QueryParserException), ExpectedMessage = "Expected Constant or Call expression for first argument of Where expression,"
        + " found MethodCallExpression (Convert(null).Where(student => True)).")]
    public void Initialize_FromWrongExpressionInWhereExpression ()
    {
      Expression nonCallExpression = Expression.Convert (Expression.Constant (null), typeof (IQueryable<Student>));
      MethodInfo method = (from m in typeof (Queryable).GetMethods () where m.Name == "Where" && m.GetParameters ().Length == 2 select m).First();
      method = method.MakeGenericMethod (typeof (Student));
      MethodCallExpression whereExpression = Expression.Call (method, nonCallExpression, Expression.Lambda (Expression.Constant(true), Expression.Parameter(typeof (Student), "student")));
      new WhereExpressionParser (whereExpression, whereExpression, true);
    }
  }
}