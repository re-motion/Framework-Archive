using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class CommitEventsTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CommitEventsTest ()
  {
  }

  // methods and properties

  [Test]
  public void CommitEvents ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    customer.Name = "New name";

    DomainObjectEventReceiver domainObjectEventReceiver = new DomainObjectEventReceiver (customer);
    ClientTransactionEventReceiver clientTransactionEventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);

    ClientTransactionMock.Commit ();

    Assert.IsTrue (domainObjectEventReceiver.HasCommittingEventBeenCalled);
    Assert.IsTrue (domainObjectEventReceiver.HasCommittedEventBeenCalled);

    Assert.AreEqual (1, clientTransactionEventReceiver.CommittingDomainObjects.Count);
    Assert.AreEqual (1, clientTransactionEventReceiver.CommittedDomainObjects.Count);

    DomainObjectCollection committingDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittingDomainObjects[0];
    DomainObjectCollection committedDomainObjects = (DomainObjectCollection) clientTransactionEventReceiver.CommittedDomainObjects[0];

    Assert.AreEqual (1, committingDomainObjects.Count);
    Assert.AreEqual (1, committedDomainObjects.Count);

    Assert.AreSame (customer, committingDomainObjects[0]);
    Assert.AreSame (customer, committedDomainObjects[0]);
  }
}
}
