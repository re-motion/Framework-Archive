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

  private QueryParameter _parameter;

  // construction and disposing

  public QueryParameterTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _parameter = new QueryParameter ("name", "value", QueryParameterType.Value);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreEqual ("name", _parameter.Name);
    Assert.AreEqual ("value", _parameter.Value);
    Assert.AreEqual (QueryParameterType.Value, _parameter.ParameterType);
  }

  [Test]
  public void SetValue ()
  {
    _parameter.Value = "NewValue";
    Assert.AreEqual ("NewValue", _parameter.Value);
  }

  [Test]
  public void SetParameterType ()
  { 
    _parameter.ParameterType = QueryParameterType.Text;
    Assert.AreEqual (QueryParameterType.Text, _parameter.ParameterType);
  }
}
}
