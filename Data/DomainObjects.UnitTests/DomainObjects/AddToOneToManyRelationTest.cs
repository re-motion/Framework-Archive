using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class AddToOneToManyRelationTest : ClientTransactionBaseTest
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

  public AddToOneToManyRelationTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
    _subordinate = Employee.GetObject (DomainObjectIDs.Employee2);

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

    _supervisor.Subordinates.Add (_subordinate);

    Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.OldRelatedObject);
    Assert.AreSame (_supervisor, _subordinateEventReceiver.NewRelatedObject);

    Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
    Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
    Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
    Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddedDomainObject);

    Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
    Assert.AreEqual ("Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
    Assert.AreSame (_subordinate, _supervisorEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Changed, _subordinate.State);
    Assert.AreEqual (StateType.Changed, _supervisor.State);
    Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
    Assert.AreSame (_supervisor, _subordinate.Supervisor);
  }


  [Test]
  public void SubordinateCancelsChangeEvent ()
  {
    _subordinateEventReceiver.Cancel = true;
    _subordinateCollectionEventReceiver.Cancel = false;
    _supervisorEventReceiver.Cancel = false;

    _supervisor.Subordinates.Add (_subordinate);

    Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.OldRelatedObject);
    Assert.AreSame (_supervisor, _subordinateEventReceiver.NewRelatedObject);

    Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
    Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
    Assert.IsNull (_subordinateCollectionEventReceiver.AddingDomainObject);
    Assert.IsNull (_subordinateCollectionEventReceiver.AddedDomainObject);

    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_supervisorEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull ( _supervisorEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
    Assert.IsNull (_supervisorEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Original, _subordinate.State);
    Assert.AreEqual (StateType.Original, _supervisor.State);
    Assert.AreEqual (2, _supervisor.Subordinates.Count);
    Assert.IsNull (_subordinate.Supervisor);
  }


  [Test]
  public void SubOrdinateCollectionCancelsChangeEvent ()
  {
    _subordinateEventReceiver.Cancel = false;
    _subordinateCollectionEventReceiver.Cancel = true;
    _supervisorEventReceiver.Cancel = false;

    _supervisor.Subordinates.Add (_subordinate);

    Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.OldRelatedObject);
    Assert.AreSame (_supervisor, _subordinateEventReceiver.NewRelatedObject);

    Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
    Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
    Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
    Assert.IsNull (_subordinateCollectionEventReceiver.AddedDomainObject);

    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsNull (_supervisorEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull ( _supervisorEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
    Assert.IsNull (_supervisorEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Original, _subordinate.State);
    Assert.AreEqual (StateType.Original, _supervisor.State);
    Assert.AreEqual (2, _supervisor.Subordinates.Count);
    Assert.IsNull (_subordinate.Supervisor);
  }

  [Test]
  public void SupervisorCancelsChangeEvent ()
  {
    _subordinateEventReceiver.Cancel = false;
    _subordinateCollectionEventReceiver.Cancel = false;
    _supervisorEventReceiver.Cancel = true;

    _supervisor.Subordinates.Add (_subordinate);

    Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_subordinateEventReceiver.OldRelatedObject);
    Assert.AreSame (_supervisor, _subordinateEventReceiver.NewRelatedObject);

    Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
    Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
    Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
    Assert.IsNull (_subordinateCollectionEventReceiver.AddedDomainObject);

    Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreEqual ("Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
    Assert.IsNull ( _supervisorEventReceiver.ChangedRelationPropertyName);
    Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
    Assert.AreSame (_subordinate, _supervisorEventReceiver.NewRelatedObject);

    Assert.AreEqual (StateType.Original, _subordinate.State);
    Assert.AreEqual (StateType.Original, _supervisor.State);
    Assert.AreEqual (2, _supervisor.Subordinates.Count);
    Assert.IsNull (_subordinate.Supervisor);
  }

  [Test]
  public void StateTracking ()
  {
    Assert.AreEqual (StateType.Original, _supervisor.State);
    Assert.AreEqual (StateType.Original, _subordinate.State);

    _supervisor.Subordinates.Add (_subordinate);

    Assert.AreEqual (StateType.Changed, _supervisor.State);
    Assert.AreEqual (StateType.Changed, _subordinate.State);
  }

  [Test]
  public void SingleObjectRelationLinkMap ()
  {
    _supervisor.Subordinates.Add (_subordinate);
    
    Assert.AreSame (_supervisor, _subordinate.Supervisor);
  }

  [Test]
  public void SetPropertyValue ()
  {
    _supervisor.Subordinates.Add (_subordinate);

    Assert.AreEqual (_supervisor.ID, _subordinate.DataContainer.GetObjectID ("Supervisor"));
  }

  [Test]
  public void SetSupervisor ()
  {
    _subordinate.Supervisor = _supervisor;
    
    Assert.AreSame (_supervisor, _subordinate.Supervisor);
    Assert.AreEqual (3, _supervisor.Subordinates.Count);
    Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
  }

  [Test]
  public void SetSameSupervisor ()
  {
    Employee employeeWithSupervisor = (Employee) _supervisor.Subordinates[DomainObjectIDs.Employee4];
    employeeWithSupervisor.Supervisor = _supervisor;

    Assert.AreEqual (StateType.Original, _supervisor.State);
    Assert.AreEqual (StateType.Original, employeeWithSupervisor.State);
  }

  [Test]
  public void SetSupervisorNull ()
  {
    _subordinate.Supervisor = null;

    // expectation: no exception
  }
}
}
