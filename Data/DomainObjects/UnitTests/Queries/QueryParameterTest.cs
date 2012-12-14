using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.UnitTests.Queries
{
[TestFixture]
public class QueryParameterTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public QueryParameterTest ()
  {
  }

  // methods and properties

  [Test]
  public void Initialize ()
  {
    QueryParameter parameter = new QueryParameter ("name", "value", QueryParameterType.Value);

    Assert.AreEqual ("name", parameter.Name);
    Assert.AreEqual ("value", parameter.Value);
    Assert.AreEqual (QueryParameterType.Value, parameter.ParameterType);
  }
}
}
