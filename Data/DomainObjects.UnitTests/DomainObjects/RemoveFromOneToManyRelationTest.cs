using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class RemoveFromOneToManyRelationTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private Employee _supervisor;
  private Employee _subordinate;

  private DomainObjectEventReceiver _supervisorEventReceiver;
  private DomainObjectEventReceiver _subordinateEventReceiver;
  private DomainObjectCollectionEventReceiver _subordinateCollectionEventReceiver;

  // construction and disposing

  public RemoveFromOneToManyRelationTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
    _subordinate = Employee.GetObject (DomainObjectIDs.Employee4);

    _supervisorEventReceiver = new DomainObjectEventReceiver (_supervisor);
    _subordinateEventReceiver = new DomainObjectEventReceiver (_subordinate);
    _subordinateCollectionEventReceiver = new DomainObjectCollectionEventReceiver (_supervisor.Subordinates);
  }

  [Test]
  public void ChangeEvents ()
  {
    _subordinateEventReceiver.Cancel = false;
    _subordinateCollectionEventReceiver.Cancel = false;
    _supervisorEventReceiver.Cancel = false;
    
    _supervisor.Subordinates.Remove (_subordinate.ID);

    Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_supervisor, _subordinateEventReceiver.OldRelatedObject);
    Assert.IsNull (_subordinateEventReceiver.NewRelatedObject);

    Assert.IsTrue (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
    Assert.IsTrue (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
    Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.RemovingDomainObjects[0]);
    Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.RemovedDomainObjects[0]);

    Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_subordinate, _supervisorEventReceiver.OldRelatedObject);
    Assert.IsNull (_supervisorEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Changed, _subordinate.State);
    Assert.AreEqual (StateType.Changed, _supervisor.State);
    Assert.IsNull (_supervisor.Subordinates[_subordinate.ID]);
    Assert.IsNull (_subordinate.Supervisor);
  }

  [Test]
  public void SubordinateCancelsChangeEvent ()
  {
    _subordinateEventReceiver.Cancel = true;
    _subordinateCollectionEventReceiver.Cancel = false;
    _supervisorEventReceiver.Cancel = false;

    _supervisor.Subordinates.Remove (_subordinate);

    Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_supervisor, _subordinateEventReceiver.OldRelatedObject);
    Assert.IsNull (_subordinateEventReceiver.NewRelatedObject);

    Assert.IsFalse (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
    Assert.IsFalse (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
    Assert.AreEqual (0, _subordinateCollectionEventReceiver.RemovingDomainObjects.Count);
    Assert.AreEqual (0, _subordinateCollectionEventReceiver.RemovedDomainObjects.Count);

    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_supervisorEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull ( _supervisorEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
    Assert.IsNull (_supervisorEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Unchanged, _subordinate.State);
    Assert.AreEqual (StateType.Unchanged, _supervisor.State);
    Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
    Assert.AreSame (_supervisor, _subordinate.Supervisor);
  }


  [Test]
  public void SubordinateCollectionCancelsChangeEvent ()
  {
    _subordinateEventReceiver.Cancel = false;
    _subordinateCollectionEventReceiver.Cancel = true;
    _supervisorEventReceiver.Cancel = false;

    _supervisor.Subordinates.Remove (_subordinate);

    Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_supervisor, _subordinateEventReceiver.OldRelatedObject);
    Assert.IsNull (_subordinateEventReceiver.NewRelatedObject);

    Assert.IsTrue (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
    Assert.IsFalse (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
    Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.RemovingDomainObjects[0]);
    Assert.AreEqual (0, _subordinateCollectionEventReceiver.RemovedDomainObjects.Count);

    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_supervisorEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull ( _supervisorEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
    Assert.IsNull (_supervisorEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Unchanged, _subordinate.State);
    Assert.AreEqual (StateType.Unchanged, _supervisor.State);
    Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
    Assert.AreSame (_supervisor, _subordinate.Supervisor);
  }
  [Test]
  public void SupervisorCancelsChangeEvent ()
  {  
    _subordinateEventReceiver.Cancel = false;
    _subordinateCollectionEventReceiver.Cancel = false;
    _supervisorEventReceiver.Cancel = true;

    _supervisor.Subordinates.Remove (_subordinate);

    Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_supervisor, _subordinateEventReceiver.OldRelatedObject);
    Assert.IsNull (_subordinateEventReceiver.NewRelatedObject);

    Assert.IsTrue (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
    Assert.IsFalse (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
    Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.RemovingDomainObjects[0]);
    Assert.AreEqual (0, _subordinateCollectionEventReceiver.RemovedDomainObjects.Count);

    Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull ( _supervisorEventReceiver.ChangedRelationPropertyName);
    Assert.AreSame (_subordinate, _supervisorEventReceiver.OldRelatedObject);
    Assert.IsNull (_supervisorEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Unchanged, _subordinate.State);
    Assert.AreEqual (StateType.Unchanged, _supervisor.State);
    Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
    Assert.AreSame (_supervisor, _subordinate.Supervisor);
  }

  [Test]
  public void StateTracking ()
  {
    Assert.AreEqual (StateType.Unchanged, _supervisor.State);
    Assert.AreEqual (StateType.Unchanged, _subordinate.State);

    _supervisor.Subordinates.Remove (_subordinate);

    Assert.AreEqual (StateType.Changed, _supervisor.State);
    Assert.AreEqual (StateType.Changed, _subordinate.State);
  }

  [Test]
  public void RelationEndPointMap ()
  {
    _supervisor.Subordinates.Remove (_subordinate.ID);
    
    Assert.IsNull (_subordinate.Supervisor);
  }

  [Test]
  public void SetPropertyValue ()
  {
    _supervisor.Subordinates.Remove (_subordinate);

    Assert.IsNull (_subordinate.DataContainer.GetObjectID ("Supervisor"));
  }

  [Test]
  public void SetSupervisorNull ()
  {
    _subordinate.Supervisor = null;
    
    Assert.IsNull (_subordinate.Supervisor);
    Assert.AreEqual (1, _supervisor.Subordinates.Count);
    Assert.IsNull (_supervisor.Subordinates[_subordinate.ID]);
  }
}
}
