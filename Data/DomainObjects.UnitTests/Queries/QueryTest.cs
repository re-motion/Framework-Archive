using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Queries
{
[TestFixture]
public class QueryTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public QueryTest ()
  {
  }

  // methods and properties

  [Test]
  public void InitializeWithQueryID ()
  {
    QueryParameterCollection parameters = new QueryParameterCollection ();
    Query query = new Query ("OrderQuery", parameters);

    QueryDefinition definition = QueryConfiguration.Current["OrderQuery"];
    Assert.AreSame (definition, query.Definition);
    Assert.AreEqual (definition.QueryID, query.QueryID);
    Assert.AreSame (parameters, query.Parameters);
  }

  [Test]
  public void InitializeWithQueryDefinition ()
  {
    QueryParameterCollection parameters = new QueryParameterCollection ();

    QueryDefinition definition = QueryFactory.CreateOrderQueryDefinition ();
    Query query = new Query (definition, parameters);

    Assert.AreSame (definition, query.Definition);
    Assert.AreEqual (definition.QueryID, query.QueryID);
    Assert.AreSame (parameters, query.Parameters);
  }
}
}
