using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Queries
{
  [TestFixture]
  public class QueryDefinitionTest : ReflectionBasedMappingTest
  {
    [Test]
    public void InitializeCollectionType ()
    {
      QueryDefinition definition = new QueryDefinition ("QueryID", "StorageProviderID", "Statement", QueryType.Collection);

      Assert.AreEqual (typeof (DomainObjectCollection), definition.CollectionType);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The scalar query 'QueryID' must not specify a collectionType.\r\nParameter name: collectionType")]
    public void InitializeScalarQueryWithCollectionType ()
    {
      QueryDefinition definition = 
          new QueryDefinition ("QueryID", "StorageProviderID", "Statement", QueryType.Scalar, typeof (DomainObjectCollection));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collectionType of query 'QueryID' must be 'Rubicon.Data.DomainObjects.DomainObjectCollection' or derived from it.\r\n"
        + "Parameter name: collectionType")]
    public void InitializeInvalidCollectionType ()
    {
      QueryDefinition definition = new QueryDefinition ("QueryID", "StorageProviderID", "Statement", QueryType.Collection, this.GetType ());
    }

    [Test]
    public void InitializeWithDomainObjectCollectionType ()
    {
      QueryDefinition definition = 
          new QueryDefinition ("QueryID", "StorageProviderID", "Statement", QueryType.Collection, typeof (DomainObjectCollection));

      Assert.AreEqual (typeof (DomainObjectCollection), definition.CollectionType);
    }
  }
}
