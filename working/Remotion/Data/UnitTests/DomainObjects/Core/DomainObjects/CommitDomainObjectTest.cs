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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class CommitDomainObjectTest : ClientTransactionBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void CommitOneToManyRelation ()
    {
      Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
      Order order = customer1.Orders[DomainObjectIDs.Order1];

      customer2.Orders.Add (order);

      Assert.AreEqual (StateType.Changed, customer1.State);
      Assert.AreEqual (StateType.Changed, customer2.State);
      Assert.AreEqual (StateType.Changed, order.State);

      TestableClientTransaction.Commit ();

      Assert.AreEqual (StateType.Unchanged, customer1.State);
      Assert.AreEqual (StateType.Unchanged, customer2.State);
      Assert.AreEqual (StateType.Unchanged, order.State);
    }

    [Test]
    public void CommitOneToOneRelation ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

			object orderTimestamp = order.InternalDataContainer.Timestamp;
			object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
			object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      TestableClientTransaction.Commit ();

			Assert.AreEqual (orderTimestamp, order.InternalDataContainer.Timestamp);
			Assert.IsFalse (oldOrderTicketTimestamp.Equals (oldOrderTicket.InternalDataContainer.Timestamp));
			Assert.IsFalse (newOrderTicketTimestamp.Equals (newOrderTicket.InternalDataContainer.Timestamp));
    }

    [Test]
    public void CommitHierarchy ()
    {
      Employee supervisor1 = Employee.GetObject (DomainObjectIDs.Employee1);
      Employee supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);
      Employee subordinate = (Employee) supervisor1.Subordinates[DomainObjectIDs.Employee4];

      subordinate.Supervisor = supervisor2;

      TestableClientTransaction.Commit ();
      ReInitializeTransaction ();

      supervisor1 = Employee.GetObject (DomainObjectIDs.Employee1);
      supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);

      Assert.IsNull (supervisor1.Subordinates[DomainObjectIDs.Employee4]);
      Assert.IsNotNull (supervisor2.Subordinates[DomainObjectIDs.Employee4]);
    }

    [Test]
    public void CommitPolymorphicRelation ()
    {
      Ceo companyCeo = Ceo.GetObject (DomainObjectIDs.Ceo1);
      Ceo distributorCeo = Ceo.GetObject (DomainObjectIDs.Ceo10);
      Company company = companyCeo.Company;
      Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor1);

      distributor.Ceo = companyCeo;
      company.Ceo = distributorCeo;

      TestableClientTransaction.Commit ();
      ReInitializeTransaction ();

      companyCeo = Ceo.GetObject (DomainObjectIDs.Ceo1);
      distributorCeo = Ceo.GetObject (DomainObjectIDs.Ceo10);
      company = Company.GetObject (DomainObjectIDs.Company1);
      distributor = Distributor.GetObject (DomainObjectIDs.Distributor1);

      Assert.AreSame (companyCeo, distributor.Ceo);
      Assert.AreSame (distributor, companyCeo.Company);
      Assert.AreSame (distributorCeo, company.Ceo);
      Assert.AreSame (company, distributorCeo.Company);
    }

    [Test]
    public void CommitPropertyChange ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      customer.Name = "Arthur Dent";

      TestableClientTransaction.Commit ();
      ReInitializeTransaction ();

      customer = Customer.GetObject (DomainObjectIDs.Customer1);
      Assert.AreEqual ("Arthur Dent", customer.Name);
    }

    [Test]
    public void OriginalDomainObjectCollection_IsNotSameAfterCommit ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DomainObjectCollection originalOrderItems = order.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      OrderItem.NewObject (order);

      TestableClientTransaction.Commit ();

      Assert.AreNotSame (originalOrderItems, order.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
      Assert.AreEqual (order.OrderItems, order.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"));
      Assert.IsTrue (order.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems").IsReadOnly);
    }
  }
}
