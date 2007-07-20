using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  public abstract class ClientTransactionStateTransitionBaseTest : ClientTransactionBaseTest
  {
    public Order GetDiscarded ()
    {
      Order discarded = Order.NewObject ();
      discarded.Delete ();
      return discarded;
    }

    public Location GetUnidirectionalWithDeletedNew ()
    {
      Location unidirectionalWithDeletedNew = Location.GetObject (DomainObjectIDs.Location3);
      unidirectionalWithDeletedNew.Client = Client.NewObject();
      unidirectionalWithDeletedNew.Client.Delete ();
      return unidirectionalWithDeletedNew;
    }

    public Location GetUnidirectionalWithDeleted ()
    {
      Location unidirectionalWithDeleted = Location.GetObject (DomainObjectIDs.Location1);
      unidirectionalWithDeleted.Client.Delete ();
      return unidirectionalWithDeleted;
    }

    public Order GetDeleted ()
    {
      Order deleted = Order.GetObject (DomainObjectIDs.Order4);
      deleted.Delete ();
      return deleted;
    }

    public Order GetNewChanged ()
    {
      Order newChanged = Order.NewObject ();
      newChanged.OrderNumber = 13;
      return newChanged;
    }

    public Order GetNewUnchanged ()
    {
      return Order.NewObject ();
    }

    public Employee GetChangedThroughRelatedObjectVirtualSide ()
    {
      Employee changedThroughRelatedObjectVirtualSide = Employee.GetObject (DomainObjectIDs.Employee3);
      changedThroughRelatedObjectVirtualSide.Computer = Computer.GetObject (DomainObjectIDs.Computer3);
      return changedThroughRelatedObjectVirtualSide;
    }

    public Computer GetChangedThroughRelatedObjectRealSide ()
    {
      Computer changedThroughRelatedObjectRealSide = Computer.GetObject (DomainObjectIDs.Computer1);
      changedThroughRelatedObjectRealSide.Employee = Employee.GetObject (DomainObjectIDs.Employee1);
      return changedThroughRelatedObjectRealSide;
    }

    public Order GetChangedThroughRelatedObjects ()
    {
      Order changedThroughRelatedObjects = Order.GetObject (DomainObjectIDs.Order3);
      changedThroughRelatedObjects.OrderItems.Clear ();
      return changedThroughRelatedObjects;
    }

    public Order GetChangedThroughPropertyValue ()
    {
      Order changedThroughPropertyValue = Order.GetObject (DomainObjectIDs.Order2);
      changedThroughPropertyValue.OrderNumber = 74;
      return changedThroughPropertyValue;
    }

    public Order GetUnchanged ()
    {
      return Order.GetObject (DomainObjectIDs.Order1);
    }

    [Test]
    public void CheckInitialStates ()
    {
      Order unchanged = GetUnchanged();
      Order changedThroughPropertyValue = GetChangedThroughPropertyValue();
      Order changedThroughRelatedObjects = GetChangedThroughRelatedObjects();
      Computer changedThroughRelatedObjectRealSide = GetChangedThroughRelatedObjectRealSide();
      Employee changedThroughRelatedObjectVirtualSide = GetChangedThroughRelatedObjectVirtualSide();
      Order newUnchanged = GetNewUnchanged();
      Order newChanged = GetNewChanged();
      Order deleted = GetDeleted();
      Location unidirectionalWithDeleted = GetUnidirectionalWithDeleted ();
      Location unidirectionalWithDeletedNew = GetUnidirectionalWithDeletedNew ();
      Order discarded = GetDiscarded();

      Assert.AreEqual (StateType.Unchanged, unchanged.State);

      Assert.AreEqual (StateType.Changed, changedThroughPropertyValue.State);
      Assert.AreNotEqual (changedThroughPropertyValue.OrderNumber,
          changedThroughPropertyValue.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int>());

      Assert.AreEqual (StateType.Changed, changedThroughRelatedObjects.State);
      Assert.AreNotEqual (changedThroughRelatedObjects.OrderItems.Count,
          changedThroughRelatedObjects.Properties[typeof (Order) + ".OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ().Count);

      Assert.AreEqual (StateType.Changed, changedThroughRelatedObjectRealSide.State);
      Assert.AreNotEqual (changedThroughRelatedObjectRealSide.Employee,
          changedThroughRelatedObjectRealSide.Properties[typeof (Computer) + ".Employee"].GetOriginalValue<Employee> ());

      Assert.AreEqual (StateType.Changed, changedThroughRelatedObjectVirtualSide.State);
      Assert.AreNotEqual (changedThroughRelatedObjectVirtualSide.Computer,
          changedThroughRelatedObjectVirtualSide.Properties[typeof (Employee) + ".Computer"].GetOriginalValue<Computer> ());

      Assert.AreEqual (StateType.New, newUnchanged.State);
      Assert.AreEqual (StateType.New, newChanged.State);

      Assert.AreEqual (StateType.Deleted, deleted.State);

      Assert.AreEqual (StateType.Unchanged, unidirectionalWithDeleted.State);
      Assert.AreEqual (StateType.Changed, unidirectionalWithDeletedNew.State);

      Assert.IsTrue (discarded.IsDiscarded);
    }
  }
}