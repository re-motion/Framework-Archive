using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Queries
{
  [TestFixture]
  public class QueryDefinitionCollectionTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private QueryDefinitionCollection _collection;
    private QueryDefinition _definition;

    // construction and disposing

    public QueryDefinitionCollectionTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _collection = new QueryDefinitionCollection ();

      _definition = new QueryDefinition (
          "OrderQuery",
          "TestDomain",
          "select Order.* from Order inner join Customer where Customer.ID = @customerID order by OrderNo asc;",
          QueryType.Collection,
          typeof (OrderCollection));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "QueryDefinition 'OrderQuery' already exists in collection.\r\nParameter name: queryDefinition")]
    public void DuplicateQueryIDs ()
    {
      _collection.Add (_definition);
      _collection.Add (_definition);
    }

    [Test]
    public void ContainsQueryDefinitionTrue ()
    {
      _collection.Add (_definition);

      Assert.IsTrue (_collection.Contains (_definition));
    }

    [Test]
    public void ContainsQueryDefinitionFalse ()
    {
      _collection.Add (_definition);

      QueryDefinition copy = new QueryDefinition (
          _definition.ID, _definition.StorageProviderID, _definition.Statement, _definition.QueryType, _definition.CollectionType);

      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullQueryDefinition ()
    {
      _collection.Contains ((QueryDefinition) null);
    }

    [Test]
    public void GetMandatory ()
    {
      _collection.Add (_definition);

      Assert.AreSame (_definition, _collection.GetMandatory (_definition.ID));
    }

    [Test]
    [ExpectedException (typeof (QueryConfigurationException),
        "QueryDefinition 'OrderQuery' does not exist.")]
    public void GetMandatoryForNonExisting ()
    {
      _collection.GetMandatory ("OrderQuery");
    }
  }
}
