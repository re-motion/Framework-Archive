using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderDeleteTest : ClientTransactionBaseTest
  {
    private SqlProvider _provider;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      RdbmsProviderDefinition definition = new RdbmsProviderDefinition (c_testDomainProviderID, typeof (SqlProvider), TestDomainConnectionString);

      _provider = new SqlProvider (definition);
    }

    public override void TearDown ()
    {
      _provider.Dispose ();
      base.TearDown ();
    }

    [Test]
    public void DeleteSingleDataContainer ()
    {
      DataContainerCollection containers = CreateDataContainerCollection (GetDeletedOrderTicketContainer());
      _provider.Connect ();
      _provider.Save (containers);

      Assert.IsNull (_provider.LoadDataContainer (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void SetTimestampOfDeletedDataContainer ()
    {
      DataContainerCollection containers = CreateDataContainerCollection (GetDeletedOrderTicketContainer ());
      _provider.Connect ();
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

			DataContainerCollection containers = CreateDataContainerCollection (supervisor.InternalDataContainer, subordinate.InternalDataContainer,
					computer.InternalDataContainer);

      _provider.Connect ();
      _provider.Save (containers);
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void ConcurrentDeleteWithForeignKey ()
    {
      ClientTransaction clientTransaction1 = new ClientTransaction ();
      ClientTransaction clientTransaction2 = new ClientTransaction ();

      OrderTicket changedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1, clientTransaction1);
      changedOrderTicket.FileName = @"C:\NewFile.jpg";

      OrderTicket deletedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1, clientTransaction2);
      deletedOrderTicket.Delete ();

      _provider.Connect ();
			_provider.Save (CreateDataContainerCollection (changedOrderTicket.InternalDataContainer));
			_provider.Save (CreateDataContainerCollection (deletedOrderTicket.InternalDataContainer));
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void ConcurrentDeleteWithoutForeignKey ()
    {
      ClientTransaction clientTransaction1 = new ClientTransaction ();
      ClientTransaction clientTransaction2 = new ClientTransaction ();

      ClassWithAllDataTypes changedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1, clientTransaction1);

      changedObject.StringProperty = "New text";

      ClassWithAllDataTypes deletedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1, clientTransaction2);

      deletedObject.Delete ();

      _provider.Connect ();
			_provider.Save (CreateDataContainerCollection (changedObject.InternalDataContainer));
			_provider.Save (CreateDataContainerCollection (deletedObject.InternalDataContainer));
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
			return orderTicket.InternalDataContainer;
    }
  }
}
