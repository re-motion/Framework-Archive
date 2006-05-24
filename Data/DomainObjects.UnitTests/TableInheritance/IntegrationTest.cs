using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class IntegrationTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public IntegrationTest ()
    {
    }

    // methods and properties

    [Test]
    public void RelationsFromConcreteSingle ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer);
      Assert.AreEqual ("UnitTests", customer.CreatedBy);
      Assert.AreEqual ("Zaphod", customer.FirstName);
      Assert.AreEqual (CustomerType.Premium, customer.CustomerType);

      Region region = customer.Region;
      Assert.IsNotNull (region);
      Assert.AreEqual (DomainObjectIDs.Region, region.ID);

      DomainObjectCollection orders = customer.Orders;
      Assert.AreEqual (1, orders.Count);
      Assert.AreEqual (DomainObjectIDs.Order, orders[0].ID);

      DomainObjectCollection historyEntries = customer.HistoryEntries;
      Assert.AreEqual (2, historyEntries.Count);
      Assert.AreEqual (DomainObjectIDs.HistoryEntry2, historyEntries[0].ID);
      Assert.AreEqual (DomainObjectIDs.HistoryEntry1, historyEntries[1].ID);

      Client client = customer.Client;
      Assert.AreEqual (DomainObjectIDs.Client, client.ID);
    }

    [Test]
    public void RelationsFromConcrete ()
    {
      Person person = Person.GetObject (DomainObjectIDs.Person);
      Assert.AreEqual (DomainObjectIDs.Client, person.Client.ID);
      Assert.AreEqual (0, person.HistoryEntries.Count);
    }


    [Test]
    public void OneToManyRelationToConcreteSingle ()
    {
      Region region = (Region) ClientTransaction.Current.GetObject (DomainObjectIDs.Region);
      Assert.AreEqual (1, region.Customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer, region.Customers[0].ID);
    }

    [Test]
    public void ManyToOneRelationToConcreteSingle ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order, new ClientTransaction ());
      Assert.AreEqual (DomainObjectIDs.Customer, order.Customer.ID);
    }

    [Test]
    public void ManyToOneRelationToAbstractClass ()
    {
      HistoryEntry historyEntry = HistoryEntry.GetObject (DomainObjectIDs.HistoryEntry1);
      Assert.AreEqual (DomainObjectIDs.Customer, historyEntry.Owner.ID);
    }

    //TODO: Client laden und zu den AssignedObjects navigieren (soll eine Person, einen Customer und eine Organizational Unit finden)
  }
}
