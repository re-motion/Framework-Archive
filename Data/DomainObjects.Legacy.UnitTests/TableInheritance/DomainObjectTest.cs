using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  [TestFixture]
  public class DomainObjectTest : TableInheritanceMappingTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void OneToManyRelationToAbstractClass ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client);

      DomainObjectCollection assignedObjects = client.AssignedObjects;

      Assert.AreEqual (3, assignedObjects.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, assignedObjects[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, assignedObjects[1].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, assignedObjects[2].ID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException),
        ExpectedMessage = "The property 'Owner' of the loaded DataContainer 'HistoryEntry|2c7fb7b3-eb16-43f9-bdde-b8b3f23a93d2|System.Guid'"
        + " refers to ClassID 'OrganizationalUnit', but the actual ClassID is 'Person'.")]
    public void SameIDInDifferentConcreteTables ()
    {
      Person person = Person.GetObject (new ObjectID (typeof (Person), new Guid ("{B969AFCB-2CDA-45ff-8490-EB52A86D5464}")));
      DomainObjectCollection historyEntries = person.HistoryEntries;
    }

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

      Assert.IsEmpty (customer.AbstractClassesWithoutDerivations);
    }

    [Test]
    public void RelationsFromConcrete ()
    {
      Person person = Person.GetObject (DomainObjectIDs.Person);
      Assert.AreEqual (DomainObjectIDs.Client, person.Client.ID);
      Assert.AreEqual (1, person.HistoryEntries.Count);
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

    [Test]
    public void UpdateConcreteSingle ()
    {
      Region expectedNewRegion = new Region ();
      expectedNewRegion.Name = "Wachau";

      Customer expectedCustomer = Customer.GetObject (DomainObjectIDs.Customer);
      expectedCustomer.LastName = "NewLastName";
      expectedCustomer.Region = expectedNewRegion;

      CommitAndReInitializeCurrentClientTransaction ();

      Customer actualCustomer = Customer.GetObject (expectedCustomer.ID);
      Assert.AreEqual ("NewLastName", actualCustomer.LastName);
      Assert.AreEqual (expectedNewRegion.ID, actualCustomer.Region.ID);
    }

    [Test]
    public void InsertConcreteSingle ()
    {
      Customer expectedCustomer = new Customer ();
      expectedCustomer.FirstName = "Franz";
      expectedCustomer.LastName = "Kameramann";
      expectedCustomer.DateOfBirth = new DateTime (1950, 1, 3);
      expectedCustomer.CustomerType = CustomerType.Premium;
      expectedCustomer.CustomerSince = DateTime.Now;
      
      Address expectedAddress = new Address ();
      expectedAddress.Street = "Linzer Straße 1";
      expectedAddress.Zip = "3100";
      expectedAddress.City = "St. Pölten";
      expectedAddress.Country = "Österreich";
      expectedAddress.Person = expectedCustomer;

      CommitAndReInitializeCurrentClientTransaction ();

      Customer actualCustomer = Customer.GetObject (expectedCustomer.ID);
      Assert.IsNotNull (actualCustomer);
      Assert.AreEqual (expectedAddress.ID, actualCustomer.Address.ID); 
    }

    [Test]
    public void DeleteConcreteSingle ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer);
      
      foreach (HistoryEntry historyEntry in customer.HistoryEntries.Clone ())
        historyEntry.Delete ();

      customer.Delete ();
      CommitAndReInitializeCurrentClientTransaction ();

      try
      {
        ClientTransaction.Current.GetObject (DomainObjectIDs.Customer);
        Assert.Fail ("ObjectNotFoundException was expected.");
      }
      catch (ObjectNotFoundException)
      {
      }
    }

    private void CommitAndReInitializeCurrentClientTransaction ()
    {
      ClientTransaction.Current.Commit ();
      ClientTransaction.SetCurrent (new ClientTransaction ());
    }
  }
}
