using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class RdbmsExpressionTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RdbmsExpressionTest ()
  {
  }

  // methods and properties

  [Test]
  public void Initialize ()
  {
    RdbmsExpression expression = new RdbmsExpression ("CustomerID asc");
    Assert.AreEqual ("CustomerID asc", expression.Text);
  }

  [Test]
  public void Check ()
  {
    RdbmsExpression expression = new RdbmsExpression ("CustomerID asc");
    expression.Check ();

    // expectation: no exception
  }

  [Test]
  [ExpectedException (typeof (RdbmsExpressionSecurityException), 
      "For security reasons the character sequence ';' must not be used in an RDBMS expression.")]
  public void CheckSemicolon ()
  {
    RdbmsExpression expression = new RdbmsExpression ("CustomerID asc;");
    expression.Check ();
  }

  [Test]
  [ExpectedException (typeof (RdbmsExpressionSecurityException), 
      "For security reasons the character sequence '--' must not be used in an RDBMS expression.")]
  public void CheckSingleLineSqlComment ()
  {
    RdbmsExpression expression = new RdbmsExpression ("truncate table Order--CustomerID asc");
    expression.Check ();
  }

  [Test]
  [ExpectedException (typeof (RdbmsExpressionSecurityException), 
      "For security reasons the character sequence '\n' must not be used in an RDBMS expression.")]
  public void CheckLinebreak ()
  {
    RdbmsExpression expression = new RdbmsExpression ("CustomerID asc\ntruncate table Order");
    expression.Check ();
  }

  [Test]
  [ExpectedException (typeof (RdbmsExpressionSecurityException), 
      "For security reasons the character sequence '\r' must not be used in an RDBMS expression.")]
  public void CheckCarriageReturn ()
  {
    RdbmsExpression expression = new RdbmsExpression ("CustomerID asc\rtruncate table Order");
    expression.Check ();
  }

  [Test]
  [ExpectedException (typeof (RdbmsExpressionSecurityException), 
      "For security reasons the character sequence '/*' must not be used in an RDBMS expression.")]
  public void CheckMultilineSqlCommentStart ()
  {
    RdbmsExpression expression = new RdbmsExpression ("/*CustomerID asc*/truncate table Order");
    expression.Check ();
  }

  [Test]
  [ExpectedException (typeof (RdbmsExpressionSecurityException), 
      "For security reasons the character sequence '*/' must not be used in an RDBMS expression.")]
  public void CheckMultilineSqlCommentEnd ()
  {
    RdbmsExpression expression = new RdbmsExpression ("CustomerID asc*/truncate table Order");
    expression.Check ();
  }
}
}
