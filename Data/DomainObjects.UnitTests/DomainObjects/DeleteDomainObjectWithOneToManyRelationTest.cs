using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.EventSequence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DeleteDomainObjectWithOneToManyRelationTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private Employee _supervisor;
  private Employee _subordinate1;
  private Employee _subordinate2;
  private SequenceEventReceiver _eventReceiver; 

  // construction and disposing

  public DeleteDomainObjectWithOneToManyRelationTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
    _subordinate1 = (Employee) _supervisor.Subordinates[0];
    _subordinate2 = (Employee) _supervisor.Subordinates[1];

    _eventReceiver = new SequenceEventReceiver (
        new DomainObject[] {_supervisor, _subordinate1, _subordinate2},
        new DomainObjectCollection[] {_supervisor.Subordinates});
  }

  [Test]
  public void DeleteSupervisor ()
  {
    _supervisor.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_supervisor, "1. Deleting of supervisor"),
      new RelationChangeState (_subordinate1, "Supervisor", _supervisor, null, "2. Relation changing of subordinate1"),
      new RelationChangeState (_subordinate2, "Supervisor", _supervisor, null, "3. Relation changing of subordinate2"),
      new ObjectDeletionState (_supervisor, "4. Deleted of supervisor"),
      new RelationChangeState (_subordinate1, "Supervisor", null, null, "5. Relation changed of subordinate1"),
      new RelationChangeState (_subordinate2, "Supervisor", null, null, "6. Relation changed of subordinate2")
    };

    _eventReceiver.Compare (expectedStates);
  }

  [Test]
  public void SupervisorCancelsDeleteEvent ()
  {
    _eventReceiver.CancelEventNumber = 1;

    _supervisor.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_supervisor, "1. Deleting of supervisor"),
    };

    _eventReceiver.Compare (expectedStates);
  }

  [Test]
  public void SubordinateCancelsDeleteEvent ()
  {
    _eventReceiver.CancelEventNumber = 2;

    _supervisor.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_supervisor, "1. Deleting of supervisor"),
      new RelationChangeState (_subordinate1, "Supervisor", _supervisor, null, "2. Relation changing of subordinate1")
    };

    _eventReceiver.Compare (expectedStates);
  }

// TODO: Implement this test: Collection must be cloned. Be aware that this conflicts with a
// many to one relation change!!!
//  [Test]
//  public void Relations ()
//  {
//    int numberOfSubordinatesBeforeDelete = _supervisor.Subordinates.Count;
//
//    _supervisor.Delete ();
//
//    Assert.AreEqual (numberOfSubordinatesBeforeDelete, _supervisor.Subordinates.Count);
//    Assert.IsNull (_subordinate1.Supervisor);
//    Assert.IsNull (_subordinate2.Supervisor);
//    Assert.IsNull (_subordinate1.DataContainer["Order"]);
//    Assert.IsNull (_subordinate2.DataContainer["Order"]);
//    Assert.AreEqual (StateType.Changed, _subordinate1.DataContainer.State);
//    Assert.AreEqual (StateType.Changed, _subordinate2.DataContainer.State);
//  }
}
}
