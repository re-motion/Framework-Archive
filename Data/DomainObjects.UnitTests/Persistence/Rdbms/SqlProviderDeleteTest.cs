using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class SqlProviderDeleteTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private SqlProvider _provider;
  private DataContainer _deletedOrderTicketContainer;

  // construction and disposing

  public SqlProviderDeleteTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    RdbmsProviderDefinition definition = new RdbmsProviderDefinition (
        c_testDomainProviderID, typeof (SqlProvider), c_connectionString);

    _provider = new SqlProvider (definition);
    _provider.Connect ();

    OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    orderTicket.Delete ();
    _deletedOrderTicketContainer = orderTicket.DataContainer;
  }

  public override void TearDown()
  {
    base.TearDown ();
    _provider.Dispose ();
  }

  [Test]
  public void DeleteSingleDataContainer ()
  {
    DataContainerCollection containers = new DataContainerCollection ();
    containers.Add (_deletedOrderTicketContainer);
    _provider.Save (containers);

    Assert.IsNull (_provider.LoadDataContainer (DomainObjectIDs.OrderTicket1));
  }

  [Test]
  public void SetTimestampOfDeletedDataContainer ()
  {
    DataContainerCollection containers = new DataContainerCollection ();
    containers.Add (_deletedOrderTicketContainer);
    _provider.Save (containers);
    _provider.SetTimestamp (containers);

    // expectation: no exception
  }
}
}
