using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Queries;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Queries
{
[TestFixture]
public class QueryDefinitionCollectionTest
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

  [SetUp]
  public void SetUp ()
  {
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
      "QueryDefinition 'OrderQuery' already exists in collection.\r\nParameter name: value")]
  public void DuplicateQueryIDs ()
  {
    _collection.Add (_definition);
    _collection.Add (_definition);
  }

  [Test]
  public void ContainsQueryDefinition ()
  {
    _collection.Add (_definition);
    
    Assert.IsTrue (_collection.Contains (_definition));    
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

    Assert.AreSame (_definition, _collection.GetMandatory (_definition.QueryID));
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
