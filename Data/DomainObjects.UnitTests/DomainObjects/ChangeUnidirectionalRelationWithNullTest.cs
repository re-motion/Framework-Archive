using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class ChangeUnidirectionalRelationWithNullTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ChangeUnidirectionalRelationWithNullTest ()
  {
  }

  // methods and properties

  [Test]
  public void SetRelatedObjectwithNewNullObject ()
  {
    Client oldClient = Client.GetObject (DomainObjectIDs.Client1);
    Location location = Location.GetObject (DomainObjectIDs.Location1);
    Assert.AreSame (oldClient, location.Client);

    location.Client = null;

    Assert.IsNull (location.Client);
    Assert.IsNull (location.DataContainer["Client"]);
    Assert.AreEqual (StateType.Changed, location.State);
    Assert.AreEqual (StateType.Unchanged, oldClient.State);
  }

  [Test]
  public void SetRelatedObjectWithOldNullObject ()
  {
    Client client = Client.GetObject (DomainObjectIDs.Client4);
    Client newClient = Client.GetObject (DomainObjectIDs.Client1);

    client.ParentClient = newClient;

    Assert.AreSame (newClient, client.ParentClient);
    Assert.AreEqual (newClient.ID, client.DataContainer["ParentClient"]);
    Assert.AreEqual (StateType.Changed, client.State);
    Assert.AreEqual (StateType.Unchanged, newClient.State);
  }

  [Test]
  public void SetRelatedObjectWithOldAndNewNullObject ()
  {
    Client client = Client.GetObject (DomainObjectIDs.Client4);
    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (client);

    client.ParentClient = null;

    eventReceiver.Check (new ChangeState[0]);
    Assert.IsNull (client.ParentClient);
    Assert.IsNull (client.DataContainer["ParentClient"]);
    Assert.AreEqual (StateType.Unchanged, client.State);
  }
}
}
