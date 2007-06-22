using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class AddToOneToManyRelationTest : ClientTransactionBaseTest
  {
    private Employee _supervisor;
    private Employee _subordinate;

    private DomainObjectEventReceiver _supervisorEventReceiver;
    private DomainObjectEventReceiver _subordinateEventReceiver;
    private DomainObjectCollectionEventReceiver _subordinateCollectionEventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      _subordinate = Employee.GetObject (DomainObjectIDs.Employee2);

      _supervisorEventReceiver = new DomainObjectEventReceiver (_supervisor);
      _subordinateEventReceiver = new DomainObjectEventReceiver (_subordinate);
      _subordinateCollectionEventReceiver = new DomainObjectCollectionEventReceiver (_supervisor.Subordinates);
    }

    [Test]
    public void AddEvents ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Add (_subordinate);

      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_subordinateEventReceiver.OldRelatedObject);
      Assert.AreSame (_supervisor, _subordinateEventReceiver.NewRelatedObject);

      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddedDomainObject);

      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
      Assert.AreSame (_subordinate, _supervisorEventReceiver.NewRelatedObject);

      Assert.AreEqual (StateType.Changed, _subordinate.State);
      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
      Assert.AreEqual (_supervisor.Subordinates.Count - 1, _supervisor.Subordinates.IndexOf (_subordinate));
      Assert.AreSame (_supervisor, _subordinate.Supervisor);
    }


    [Test]
    public void SubordinateCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = true;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
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
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
        Assert.IsNull (_supervisorEventReceiver.NewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.AreEqual (2, _supervisor.Subordinates.Count);
        Assert.IsNull (_subordinate.Supervisor);
      }
    }


    [Test]
    public void SubOrdinateCollectionCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = true;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
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
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
        Assert.IsNull (_supervisorEventReceiver.NewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.AreEqual (2, _supervisor.Subordinates.Count);
        Assert.IsNull (_subordinate.Supervisor);
      }
    }

    [Test]
    public void SupervisorCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = true;

      try
      {
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.OldRelatedObject);
        Assert.AreSame (_supervisor, _subordinateEventReceiver.NewRelatedObject);

        Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
        Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
        Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
        Assert.IsNull (_subordinateCollectionEventReceiver.AddedDomainObject);

        Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
        Assert.AreSame (_subordinate, _supervisorEventReceiver.NewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.AreEqual (2, _supervisor.Subordinates.Count);
        Assert.IsNull (_subordinate.Supervisor);
      }
    }

    [Test]
    public void StateTracking ()
    {
      Assert.AreEqual (StateType.Unchanged, _supervisor.State);
      Assert.AreEqual (StateType.Unchanged, _subordinate.State);

      _supervisor.Subordinates.Add (_subordinate);

      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.AreEqual (StateType.Changed, _subordinate.State);
    }

    [Test]
    public void RelationEndPointMap ()
    {
      _supervisor.Subordinates.Add (_subordinate);

      Assert.AreSame (_supervisor, _subordinate.Supervisor);
    }

    [Test]
    public void SetPropertyValue ()
    {
      _supervisor.Subordinates.Add (_subordinate);

			Assert.AreEqual (_supervisor.ID, _subordinate.InternalDataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
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
      Employee employeeWithSupervisor = _supervisor.Subordinates[DomainObjectIDs.Employee4];
      employeeWithSupervisor.Supervisor = _supervisor;

      Assert.AreEqual (StateType.Unchanged, _supervisor.State);
      Assert.AreEqual (StateType.Unchanged, employeeWithSupervisor.State);
    }

    [Test]
    public void SetSupervisorNull ()
    {
      _subordinate.Supervisor = null;

      // expectation: no exception
    }

    [Test]
    public void InsertEvents ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Insert (0, _subordinate);

      Assert.AreEqual (0, _supervisor.Subordinates.IndexOf (_subordinate));
      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_subordinateEventReceiver.OldRelatedObject);
      Assert.AreSame (_supervisor, _subordinateEventReceiver.NewRelatedObject);

      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddedDomainObject);

      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
      Assert.AreSame (_subordinate, _supervisorEventReceiver.NewRelatedObject);

      Assert.AreEqual (StateType.Changed, _subordinate.State);
      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
      Assert.AreSame (_supervisor, _subordinate.Supervisor);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Cannot add object 'Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid'"
        + " already part of this collection.\r\nParameter name: domainObject")]
    public void AddObjectAlreadyInCollection ()
    {
      _supervisor.Subordinates.Add (_subordinate);
      _supervisor.Subordinates.Add (_subordinate);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Cannot insert object 'Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid'"
        + " already part of this collection.\r\nParameter name: domainObject")]
    public void InsertObjectAlreadyInCollection ()
    {
      _supervisor.Subordinates.Insert (0, _subordinate);
      _supervisor.Subordinates.Insert (0, _subordinate);
    }

    [Test]
    public void AddSubordinateWithOldSupervisor ()
    {
      Employee subordinate = Employee.GetObject (DomainObjectIDs.Employee3);
      Employee oldSupervisorOfSubordinate = Employee.GetObject (DomainObjectIDs.Employee2);

      DomainObjectEventReceiver subordinateEventReceiver = new DomainObjectEventReceiver (subordinate);
      subordinateEventReceiver.Cancel = false;

      DomainObjectEventReceiver oldSupervisorEventReceiver = new DomainObjectEventReceiver (oldSupervisorOfSubordinate);
      oldSupervisorEventReceiver.Cancel = false;

      DomainObjectCollectionEventReceiver oldSupervisorSubordinateCollectionEventReceiver =
          new DomainObjectCollectionEventReceiver (oldSupervisorOfSubordinate.Subordinates);
      oldSupervisorSubordinateCollectionEventReceiver.Cancel = false;

      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Add (subordinate);

      Assert.IsTrue (oldSupervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (oldSupervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", oldSupervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", oldSupervisorEventReceiver.ChangedRelationPropertyName);

      Assert.IsTrue (oldSupervisorSubordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
      Assert.IsTrue (oldSupervisorSubordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
      Assert.AreEqual (1, oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects.Count);
      Assert.AreSame (subordinate, oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects[0]);
      Assert.AreEqual (1, oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects.Count);
      Assert.AreSame (subordinate, oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects[0]);


      Assert.IsTrue (subordinateEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (subordinateEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", subordinateEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", subordinateEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (oldSupervisorOfSubordinate, subordinateEventReceiver.OldRelatedObject);
      Assert.AreSame (_supervisor, subordinateEventReceiver.NewRelatedObject);

      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
      Assert.AreSame (subordinate, _subordinateCollectionEventReceiver.AddedDomainObject);

      Assert.IsTrue (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_supervisorEventReceiver.OldRelatedObject);
      Assert.AreSame (subordinate, _supervisorEventReceiver.NewRelatedObject);

      Assert.AreEqual (StateType.Changed, subordinate.State);
      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.AreEqual (StateType.Changed, oldSupervisorOfSubordinate.State);

      Assert.IsNotNull (_supervisor.Subordinates[subordinate.ID]);
      Assert.AreEqual (_supervisor.Subordinates.Count - 1, _supervisor.Subordinates.IndexOf (subordinate));
      Assert.IsFalse (oldSupervisorOfSubordinate.Subordinates.ContainsObject (subordinate));
      Assert.AreSame (_supervisor, subordinate.Supervisor);
    }
  }
}
