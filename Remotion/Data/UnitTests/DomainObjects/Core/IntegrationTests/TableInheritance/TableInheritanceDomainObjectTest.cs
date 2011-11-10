// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.TableInheritance
{
  [TestFixture]
  public class TableInheritanceDomainObjectTest : TableInheritanceMappingTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void OneToManyRelationToAbstractClass ()
    {
      TIClient client = TIClient.GetObject (DomainObjectIDs.Client);

      DomainObjectCollection assignedObjects = client.AssignedObjects;

      Assert.AreEqual (4, assignedObjects.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, assignedObjects[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, assignedObjects[1].ID);
      Assert.AreEqual (DomainObjectIDs.PersonForUnidirectionalRelationTest, assignedObjects[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, assignedObjects[3].ID);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage =
        "The property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance.TIHistoryEntry.Owner' of the loaded DataContainer "
        + "'TI_HistoryEntry|2c7fb7b3-eb16-43f9-bdde-b8b3f23a93d2|System.Guid' refers to ClassID 'TI_OrganizationalUnit', "
        + "but the actual ClassID is 'TI_Person'.")]
    public void SameIDInDifferentConcreteTables ()
    {
      TIPerson person = TIPerson.GetObject (new ObjectID (typeof (TIPerson), new Guid ("{B969AFCB-2CDA-45ff-8490-EB52A86D5464}")));
      DomainObjectCollection historyEntries = person.HistoryEntries;
    }

    [Test]
    public void RelationsFromConcreteSingle ()
    {
      TICustomer customer = TICustomer.GetObject (DomainObjectIDs.Customer);
      Assert.AreEqual ("UnitTests", customer.CreatedBy);
      Assert.AreEqual ("Zaphod", customer.FirstName);
      Assert.AreEqual (CustomerType.Premium, customer.CustomerType);

      TIRegion region = customer.Region;
      Assert.IsNotNull (region);
      Assert.AreEqual (DomainObjectIDs.Region, region.ID);

      DomainObjectCollection orders = customer.Orders;
      Assert.AreEqual (1, orders.Count);
      Assert.AreEqual (DomainObjectIDs.Order, orders[0].ID);

      DomainObjectCollection historyEntries = customer.HistoryEntries;
      Assert.AreEqual (2, historyEntries.Count);
      Assert.AreEqual (DomainObjectIDs.HistoryEntry2, historyEntries[0].ID);
      Assert.AreEqual (DomainObjectIDs.HistoryEntry1, historyEntries[1].ID);

      TIClient client = customer.Client;
      Assert.AreEqual (DomainObjectIDs.Client, client.ID);

      Assert.IsEmpty (customer.AbstractClassesWithoutDerivations);
    }

    [Test]
    public void RelationsFromConcrete ()
    {
      TIPerson person = TIPerson.GetObject (DomainObjectIDs.Person);
      Assert.AreEqual (DomainObjectIDs.Client, person.Client.ID);
      Assert.AreEqual (1, person.HistoryEntries.Count);
    }


    [Test]
    public void OneToManyRelationToConcreteSingle ()
    {
      TIRegion region = TIRegion.GetObject (DomainObjectIDs.Region);
      Assert.AreEqual (1, region.Customers.Count);
      Assert.AreEqual (DomainObjectIDs.Customer, region.Customers[0].ID);
    }

    [Test]
    public void ManyToOneRelationToConcreteSingle ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        TIOrder order = TIOrder.GetObject (DomainObjectIDs.Order);
        Assert.AreEqual (DomainObjectIDs.Customer, order.Customer.ID);
      }
    }

    [Test]
    public void ManyToOneRelationToAbstractClass ()
    {
      TIHistoryEntry historyEntry = TIHistoryEntry.GetObject (DomainObjectIDs.HistoryEntry1);
      Assert.AreEqual (DomainObjectIDs.Customer, historyEntry.Owner.ID);
    }

    [Test]
    public void UpdateConcreteSingle ()
    {
      TIRegion expectedNewRegion = TIRegion.NewObject ();
      expectedNewRegion.Name = "Wachau";

      TICustomer expectedCustomer = TICustomer.GetObject (DomainObjectIDs.Customer);
      expectedCustomer.LastName = "NewLastName";
      expectedCustomer.Region = expectedNewRegion;

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        TICustomer actualCustomer = TICustomer.GetObject (expectedCustomer.ID);
        Assert.AreEqual ("NewLastName", actualCustomer.LastName);
        Assert.AreEqual (expectedNewRegion.ID, actualCustomer.Region.ID);
      }
    }

    [Test]
    public void InsertConcreteSingle ()
    {
      TICustomer expectedCustomer = TICustomer.NewObject ();
      expectedCustomer.FirstName = "Franz";
      expectedCustomer.LastName = "Kameramann";
      expectedCustomer.DateOfBirth = new DateTime (1950, 1, 3);
      expectedCustomer.CustomerType = CustomerType.Premium;
      expectedCustomer.CustomerSince = DateTime.Now;

      TIAddress expectedAddress = TIAddress.NewObject();
      expectedAddress.Street = "Linzer Stra�e 1";
      expectedAddress.Zip = "3100";
      expectedAddress.City = "St. P�lten";
      expectedAddress.Country = "�sterreich";
      expectedAddress.Person = expectedCustomer;

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        TICustomer actualCustomer = TICustomer.GetObject (expectedCustomer.ID);
        Assert.IsNotNull (actualCustomer);
        Assert.AreEqual (expectedAddress.ID, actualCustomer.Address.ID);
      }
    }

    [Test]
    public void DeleteConcreteSingle ()
    {
      var customer = TICustomer.GetObject (DomainObjectIDs.Customer);
      
      
      foreach (var historyEntry in customer.HistoryEntries.Clone ())
        historyEntry.Delete ();

      customer.Delete ();
      

      ClientTransactionScope.CurrentTransaction.Commit ();
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        try
        {
          TICustomer.GetObject (DomainObjectIDs.Customer);
          Assert.Fail ("ObjectNotFoundException was expected.");
        }
        catch (ObjectNotFoundException)
        {
        }
      }
    }
  }
}
