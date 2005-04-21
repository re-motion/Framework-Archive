using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class UnidirectionalRelationTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private Client _oldClient;
  private Client _newClient;
  private Location _location;

  // construction and disposing

  public UnidirectionalRelationTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _oldClient = Client.GetObject (DomainObjectIDs.Client1);
    _newClient = Client.GetObject (DomainObjectIDs.Client2);
    _location = Location.GetObject (DomainObjectIDs.Location1);
  }

  [Test]
  public void SetRelatedObject ()
  {
    _location.Client = _newClient;
    
    Assert.AreSame (_newClient, _location.Client);
    Assert.AreEqual (_newClient.ID, _location.DataContainer["Client"]);
    Assert.AreEqual (StateType.Changed, _location.State);
    Assert.AreEqual (StateType.Unchanged, _oldClient.State);
    Assert.AreEqual (StateType.Unchanged, _newClient.State);
  }

  [Test]
  public void EventsForSetRelatedObject ()
  {
    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] {_location, _oldClient, _newClient}, new DomainObjectCollection[0]);

    _location.Client = _newClient;
    
    ChangeState[] expectedStates = new ChangeState[]
    {
      new RelationChangeState (_location, "Client", _oldClient, _newClient, "1. Changing event of location"),
      new RelationChangeState (_location, "Client", null, null, "2. Changed event of location")
    };
  
    eventReceiver.Check (expectedStates);
  }

  [Test]
  public void SetRelatedObjectWithSameOldAndNewObject ()
  {
    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] {_location, _oldClient, _newClient}, new DomainObjectCollection[0]);

    _location.Client = _oldClient;
    
    eventReceiver.Check (new ChangeState[0]);
    Assert.AreEqual (StateType.Unchanged, _location.State);
  }

  [Test]
  public void GetRelatedObject ()
  {
    Assert.AreSame (_oldClient, _location.GetRelatedObject ("Client"));
  }

  [Test]
  public void GetOriginalRelatedObject ()
  {
    Assert.AreSame (_oldClient, _location.GetOriginalRelatedObject ("Client"));

    _location.Client = _newClient;

    Assert.AreSame (_oldClient, _location.GetOriginalRelatedObject ("Client"));
  }

  [Test]
  public void CreateObjectsAndCommit ()
  {
    Client client1 = new Client ();
    Client client2 = new Client ();
    Location location = new Location ();

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] {location, client1, client2}, new DomainObjectCollection[0]);
    
    location.Client = client1;

    Assert.AreEqual (StateType.New, client1.State);
    Assert.AreEqual (StateType.New, client2.State);
    Assert.AreEqual (StateType.New, location.State);

    ObjectID clientID1 = client1.ID;
    ObjectID clientID2 = client2.ID;
    ObjectID locationID = location.ID;

    
    ChangeState[] expectedStates = new ChangeState[]
    {
      new RelationChangeState (location, "Client", null, client1, "1. Changing event of location"),
      new RelationChangeState (location, "Client", null, null, "2. Changed event of location")
    };
  
    eventReceiver.Check (expectedStates);

    ClientTransactionMock.Commit ();
    
    ClientTransaction otherClientTransaction = new ClientTransaction ();
    client1 = (Client) otherClientTransaction.GetObject (clientID1);
    client2 = (Client) otherClientTransaction.GetObject (clientID2);
    location = (Location) otherClientTransaction.GetObject (locationID);

    Assert.IsNotNull (client1);
    Assert.IsNotNull (client2);
    Assert.IsNotNull (location);
    Assert.AreSame (client1, location.Client);
  }

  [Test]
  public void DeleteLocationAndCommit ()
  {
    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] {_location, _oldClient, _newClient}, new DomainObjectCollection[0]);

    _location.Delete ();
    ClientTransactionMock.Commit ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_location, "1. Deleting event of location"),
      new ObjectDeletionState (_location, "2. Deleted event of location")
    };

    eventReceiver.Check (expectedStates);
  }

  [Test]
  public void DeleteMultipleObjectsAndCommit ()
  {
    _location.Delete ();
    _oldClient.Delete ();
    _newClient.Delete ();
    
    Client client3 = Client.GetObject (DomainObjectIDs.Client3);
    client3.Delete ();

    Location location2 = Location.GetObject (DomainObjectIDs.Location2);
    location2.Delete ();

    Location location3 = Location.GetObject (DomainObjectIDs.Location3);
    location3.Delete ();

    ClientTransactionMock.Commit ();
  }

  [Test]
  public void Rollback ()
  {
    _location.Delete ();
    Location newLocation = new Location ();
    newLocation.Client = _newClient;

    ClientTransactionMock.Rollback ();

    Assert.AreEqual (StateType.Unchanged, _location.State);
  }

  [Test]
  public void CreateHierarchy ()
  {
    Client newClient1 = new Client ();
    Client newClient2 = new Client ();
    newClient2.ParentClient = newClient1;

    ObjectID newClientID1 = newClient1.ID;
    ObjectID newClientID2 = newClient2.ID;

    ClientTransactionMock.Commit ();

    ClientTransaction otherClientTransaction = new ClientTransaction ();
    newClient1 = (Client) otherClientTransaction.GetObject (newClientID1);
    newClient2 = (Client) otherClientTransaction.GetObject (newClientID2);

    Assert.IsNotNull (newClient1);
    Assert.IsNotNull (newClient2);
    Assert.AreSame (newClient1, newClient2.ParentClient);
  }
}
}
