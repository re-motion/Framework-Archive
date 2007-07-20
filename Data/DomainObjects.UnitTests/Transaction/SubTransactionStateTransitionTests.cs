using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class SubTransactionStateTransitionTests : ClientTransactionBaseTest
  {
    private static Order GetDiscarded ()
    {
      Order discarded = Order.NewObject ();
      discarded.Delete ();
      return discarded;
    }

    private Location GetUnidirectionalWithDeletedNew ()
    {
      Location unidirectionalWithDeletedNew = Location.GetObject (DomainObjectIDs.Location3);
      unidirectionalWithDeletedNew.Client = Client.NewObject();
      unidirectionalWithDeletedNew.Client.Delete ();
      return unidirectionalWithDeletedNew;
    }

    private Location GetUnidirectionalWithDeleted ()
    {
      Location unidirectionalWithDeleted = Location.GetObject (DomainObjectIDs.Location1);
      unidirectionalWithDeleted.Client.Delete ();
      return unidirectionalWithDeleted;
    }

    private Order GetDeleted ()
    {
      Order deleted = Order.GetObject (DomainObjectIDs.Order4);
      deleted.Delete ();
      return deleted;
    }

    private Order GetNewChanged ()
    {
      Order newChanged = Order.NewObject ();
      newChanged.OrderNumber = 13;
      return newChanged;
    }

    private Order GetNewUnchanged ()
    {
      return Order.NewObject ();
    }

    private Employee GetChangedThroughRelatedObjectVirtualSide ()
    {
      Employee changedThroughRelatedObjectVirtualSide = Employee.GetObject (DomainObjectIDs.Employee3);
      changedThroughRelatedObjectVirtualSide.Computer = Computer.GetObject (DomainObjectIDs.Computer3);
      return changedThroughRelatedObjectVirtualSide;
    }

    private Computer GetChangedThroughRelatedObjectRealSide ()
    {
      Computer changedThroughRelatedObjectRealSide = Computer.GetObject (DomainObjectIDs.Computer1);
      changedThroughRelatedObjectRealSide.Employee = Employee.GetObject (DomainObjectIDs.Employee1);
      return changedThroughRelatedObjectRealSide;
    }

    private Order GetChangedThroughRelatedObjects ()
    {
      Order changedThroughRelatedObjects = Order.GetObject (DomainObjectIDs.Order3);
      changedThroughRelatedObjects.OrderItems.Clear ();
      return changedThroughRelatedObjects;
    }

    private Order GetChangedThroughPropertyValue ()
    {
      Order changedThroughPropertyValue = Order.GetObject (DomainObjectIDs.Order2);
      changedThroughPropertyValue.OrderNumber = 74;
      return changedThroughPropertyValue;
    }

    private Order GetUnchanged ()
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

    [Test]
    public void RootToSubUnchanged ()
    {
      DomainObject obj = GetUnchanged ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughPropertyValue ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.AreEqual (obj.OrderNumber,
            obj.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int> ());
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjects ()
    {
      Order obj = GetChangedThroughRelatedObjects ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.AreEqual (obj.OrderItems.Count,
           obj.Properties[typeof (Order) + ".OrderItems"].GetOriginalValue<ObjectList<OrderItem>> ().Count);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjectRealSide ()
    {
      Computer obj = GetChangedThroughRelatedObjectRealSide ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.AreEqual (obj.Employee,
            obj.Properties[typeof (Computer) + ".Employee"].GetOriginalValue<Employee> ());
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjectVirtualSide ()
    {
      Employee obj = GetChangedThroughRelatedObjectVirtualSide ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.AreEqual (obj.Computer,
            obj.Properties[typeof (Employee) + ".Computer"].GetOriginalValue<Computer> ());
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void RootToSubNewUnchanged ()
    {
      DomainObject obj = GetNewUnchanged ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void RootToSubNewChanged ()
    {
      DomainObject obj = GetNewChanged ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void RootToSubDeleted ()
    {
      Order obj = GetDeleted ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.IsTrue (obj.IsDiscarded);
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' could not be found.")]
    public void RootToSubDeletedThrowsWhenReloadingTheObject ()
    {
      Order obj = GetDeleted ();
      ObjectID id = obj.ID;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.IsTrue (obj.IsDiscarded);
        Order.GetObject (id);
      }
    }

    [Test]
    public void RootToSubUnidirectionalWithDeleted ()
    {
      Client deleted = Client.GetObject (DomainObjectIDs.Client1);
      Location obj = GetUnidirectionalWithDeleted ();
      Assert.AreEqual (StateType.Deleted, deleted.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        Assert.IsTrue (deleted.IsDiscarded);
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
      Assert.AreEqual (StateType.Deleted, deleted.State);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Client|1627ade8-125f-4819-8e33-ce567c42b00c|System.Guid' could not be found.")]
    public void RootToSubUnidirectionalWithDeletedThrowsWhenAccessingTheObject ()
    {
      Location obj = GetUnidirectionalWithDeleted ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Client client = obj.Client;
      }
    }

    [Test]
    public void RootToSubUnidirectionalWithDeletedNew ()
    {
      Location obj = GetUnidirectionalWithDeletedNew ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Client|.*|System.Guid' could not be found.", MatchType = MessageMatch.Regex)]
    public void RootToSubUnidirectionalWithDeletedNewThrowsWhenAccessingTheObject ()
    {
      Location obj = GetUnidirectionalWithDeletedNew ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Client client = obj.Client;
      }
    }

    [Test]
    public void RootToSubDiscarded ()
    {
      DomainObject obj = GetDiscarded ();
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Assert.IsTrue (obj.IsDiscarded);
      }
      Assert.IsTrue (obj.IsDiscarded);
    }
  }
}