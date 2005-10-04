using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Queries
{
[TestFixture]
public class QueryDefinitionTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public QueryDefinitionTest ()
  {
  }

  // methods and properties

  [Test]
  public void InitializeCollectionType ()
  {
    QueryDefinition definition = new QueryDefinition (
        "QueryID", "StorageProviderID", "Statement", QueryType.Collection);

    Assert.AreEqual (typeof (DomainObjectCollection), definition.CollectionType);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), "The scalar query 'QueryID' must not specify a collectionType.\r\nParameter name: collectionType")]
  public void InitializeScalarQueryWithCollectionType ()
  {
    QueryDefinition definition = new QueryDefinition (
        "QueryID", "StorageProviderID", "Statement", QueryType.Scalar, typeof (DomainObjectCollection));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), "The collectionType of query 'QueryID' must be 'Rubicon.Data.DomainObjects.DomainObjectCollection' or derived from it.\r\nParameter name: collectionType")]
  public void InitializeInvalidCollectionType ()
  {
    QueryDefinition definition = new QueryDefinition (
        "QueryID", "StorageProviderID", "Statement", QueryType.Collection, this.GetType ());
  }

  [Test]
  public void InitializeWithDomainObjectCollectionType ()
  {
    QueryDefinition definition = new QueryDefinition (
        "QueryID", "StorageProviderID", "Statement", QueryType.Collection, typeof (DomainObjectCollection));

    Assert.AreEqual (typeof (DomainObjectCollection), definition.CollectionType);    
  }
}
}
