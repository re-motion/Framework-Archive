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
  }

  public override void TearDown()
  {
    base.TearDown ();
    _provider.Dispose ();
  }

  [Test]
  public void DeleteSingleDataContainer ()
  {
    _provider.Save (CreateDataContainerCollection (GetDeletedOrderTicketContainer ()));

    Assert.IsNull (_provider.LoadDataContainer (DomainObjectIDs.OrderTicket1));
  }

  [Test]
  public void SetTimestampOfDeletedDataContainer ()
  {
    DataContainerCollection containers = CreateDataContainerCollection (GetDeletedOrderTicketContainer ());
    _provider.Save (containers);
    _provider.SetTimestamp (containers);

    // expectation: no exception
  }

  [Test]
  public void DeleteRelatedDataContainers ()
  {
    Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee2);
    Employee subordinate = Employee.GetObject (DomainObjectIDs.Employee3);
    Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);

    supervisor.Delete ();
    subordinate.Delete ();
    computer.Delete ();

    DataContainerCollection containers = 
        CreateDataContainerCollection (supervisor.DataContainer, subordinate.DataContainer, computer.DataContainer);

    _provider.Save (containers);
  }

  [Test]
  [ExpectedException (typeof (ConcurrencyViolationException))]
  public void ConcurrentDeleteWithForeignKey ()
  {
    ClientTransactionMock clientTransaction1 = new ClientTransactionMock ();
    ClientTransactionMock clientTransaction2 = new ClientTransactionMock ();

    ClientTransaction.SetCurrent (clientTransaction1);
    OrderTicket changedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    changedOrderTicket.FileName = @"C:\NewFile.jpg";
  
    ClientTransaction.SetCurrent (clientTransaction2);
    OrderTicket deletedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    deletedOrderTicket.Delete ();

    _provider.Save (CreateDataContainerCollection (changedOrderTicket.DataContainer));
    _provider.Save (CreateDataContainerCollection (deletedOrderTicket.DataContainer));
  }

  [Test]
  [ExpectedException (typeof (ConcurrencyViolationException))]
  public void ConcurrentDeleteWithoutForeignKey ()
  {
    ClientTransactionMock clientTransaction1 = new ClientTransactionMock ();
    ClientTransactionMock clientTransaction2 = new ClientTransactionMock ();

    ClientTransaction.SetCurrent (clientTransaction1);
    ClassWithAllDataTypes changedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
    changedObject.StringProperty = "New text";
  
    ClientTransaction.SetCurrent (clientTransaction2);
    ClassWithAllDataTypes deletedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
    deletedObject.Delete ();

    _provider.Save (CreateDataContainerCollection (changedObject.DataContainer));
    _provider.Save (CreateDataContainerCollection (deletedObject.DataContainer));
  }

  private DataContainerCollection CreateDataContainerCollection (params DataContainer[] dataContainers)
  {
    DataContainerCollection collection = new DataContainerCollection ();
    foreach (DataContainer dataContainer in dataContainers)
      collection.Add (dataContainer);

    return collection;
  }

  private DataContainer GetDeletedOrderTicketContainer ()
  {
    OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    orderTicket.Delete ();
    return orderTicket.DataContainer;
  }
}
}
