using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class SqlProviderQueryTest: SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderQueryTest ()
  {
  }

  // methods and properties

  [Test]
  public void GetValueWithoutParameter ()
  {
    //Assert.AreEqual (42, Provider.ExecuteScalarQuery (new Query ("QueryWithoutParameter")));
  }
}
}
