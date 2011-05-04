// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ClientTransactionTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private DataManager _dataManager;

    private MockRepository _mockRepository;
    private Dictionary<Enum, object> _fakeApplicationData;
    private IDataManager _dataManagerMock;
    private IEnlistedDomainObjectManager _enlistedObjectManagerMock;
    private ClientTransactionExtensionCollection _fakeExtensions;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;
    private CompoundClientTransactionListener _fakeListeners;
    private IObjectLoader _objectLoaderMock;
    private IPersistenceStrategy _persistenceStrategyMock;

    private ClientTransaction _transactionWithMocks;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction ();
      _dataManager = ClientTransactionTestHelper.GetDataManager (_transaction);

      _mockRepository = new MockRepository();
      _fakeApplicationData = new Dictionary<Enum, object>();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();
      _enlistedObjectManagerMock = _mockRepository.StrictMock<IEnlistedDomainObjectManager> ();
      _fakeExtensions = new ClientTransactionExtensionCollection();
      _invalidDomainObjectManagerMock = _mockRepository.StrictMock<IInvalidDomainObjectManager> ();
      _fakeListeners = new CompoundClientTransactionListener();
      _objectLoaderMock = _mockRepository.StrictMock<IObjectLoader> ();
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();

      _transactionWithMocks = ClientTransactionObjectMother.Create (
          _fakeApplicationData,
          tx => { throw new NotImplementedException(); },
          _dataManagerMock,
          _enlistedObjectManagerMock,
          _fakeExtensions,
          _invalidDomainObjectManagerMock,
          new[] { _fakeListeners },
          _objectLoaderMock,
          _persistenceStrategyMock);
    }

    [Test]
    public void ParentTransaction ()
    {
      var parent = ClientTransaction.CreateRootTransaction();
      _persistenceStrategyMock.Stub (stub => stub.ParentTransaction).Return (parent);
      _mockRepository.ReplayAll();
      
      Assert.That (_transactionWithMocks.ParentTransaction, Is.SameAs (parent));
    }

    [Test]
    public void ParentTransaction_Null ()
    {
      _persistenceStrategyMock.Stub (stub => stub.ParentTransaction).Return (null);
      _mockRepository.ReplayAll ();

      Assert.That (_transactionWithMocks.ParentTransaction, Is.Null);
    }

    [Test]
    public void ActiveSubTransaction_Null ()
    {
      Assert.That (_transactionWithMocks.SubTransaction, Is.Null);
    }

    [Test]
    public void RootTransaction_Same ()
    {
      _persistenceStrategyMock.Stub (stub => stub.ParentTransaction).Return (null);
      _mockRepository.ReplayAll();
      
      Assert.That (_transactionWithMocks.RootTransaction, Is.SameAs (_transactionWithMocks));
    }

    [Test]
    public void RootTransaction_NotSame ()
    {
      var grandParentTransaction = ClientTransaction.CreateRootTransaction ();
      var parentTransaction = grandParentTransaction.CreateSubTransaction();
      _persistenceStrategyMock.Stub (stub => stub.ParentTransaction).Return (parentTransaction);
      _mockRepository.ReplayAll ();

      Assert.That (_transactionWithMocks.RootTransaction, Is.SameAs (grandParentTransaction));
    }

    [Test]
    public void LeafTransaction_Same ()
    {
      Assert.That (_transaction.LeafTransaction, Is.SameAs (_transaction));
    }

    [Test]
    public void LeafTransaction_NotSame ()
    {
      var subTransaction1 = _transaction.CreateSubTransaction ();
      var subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (_transaction.LeafTransaction, Is.SameAs (subTransaction2));
    }

    [Test]
    public void GetObject_UnknownObject_IsLoaded ()
    {
      var result = ClientTransactionTestHelper.CallGetObject (_transaction, DomainObjectIDs.Order1, false);

      Assert.That (result, Is.InstanceOf (typeof (Order)));
      Assert.That (result.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (result.TransactionContext[_transaction].State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetObject_KnownObject_IsReturned ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());

      var result = ClientTransactionTestHelper.CallGetObject (_transaction, order.ID, false);

      Assert.That (result, Is.SameAs (order));
    }

    [Test]
    public void GetObject_EnlistedButNotLoadedObject_LoadsObject ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      _transaction.EnlistDomainObject (order);

      Assert.That (order.TransactionContext[_transaction].State, Is.EqualTo (StateType.NotLoadedYet));

      var result = ClientTransactionTestHelper.CallGetObject (_transaction, order.ID, false);

      Assert.That (result, Is.SameAs (order));
      Assert.That (result.TransactionContext[_transaction].State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void GetObject_InvalidObject_ThrowsWithoutGoingToDatabase ()
    {
      var invalidObject = _transaction.Execute (() => Official.NewObject ());
      LifetimeService.DeleteObject (_transaction, invalidObject);

      Assert.That (invalidObject.TransactionContext[_transaction].IsInvalid, Is.True);

      var storageProviderMock = UnitTestStorageProviderStub.CreateStorageProviderMockForOfficial ();
      try
      {
        UnitTestStorageProviderStub.ExecuteWithMock (storageProviderMock, () => 
            ClientTransactionTestHelper.CallGetObject (_transaction, invalidObject.ID, false));
        Assert.Fail ("Expected ObjectNotFoundException.");
      }
      catch (ObjectInvalidException)
      {
        // ok
      }

      storageProviderMock.AssertWasNotCalled (mock => mock.LoadDataContainer (invalidObject.ID));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObject_DeletedObject_IncludeDeletedFalse_Throws ()
    {
      var deletedObject = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      LifetimeService.DeleteObject (_transaction, deletedObject);

      Assert.That (deletedObject.TransactionContext[_transaction].State, Is.EqualTo (StateType.Deleted));

      ClientTransactionTestHelper.CallGetObject (_transaction, deletedObject.ID, false);
    }

    [Test]
    public void GetObject_DeletedObject_IncludeDeletedTrue_Returns ()
    {
      var deletedObject = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      LifetimeService.DeleteObject (_transaction, deletedObject);

      Assert.That (deletedObject.TransactionContext[_transaction].State, Is.EqualTo (StateType.Deleted));

      var result = ClientTransactionTestHelper.CallGetObject (_transaction, deletedObject.ID, true);

      Assert.That (result, Is.SameAs (deletedObject));
    }

    [Test]
    public void GetObjectReference_KnownObject_ReturnedWithoutLoading ()
    {
      var instance = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      _transaction.EnlistDomainObject (instance);

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvent (
          _transaction,
          mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));

      var result = ClientTransactionTestHelper.CallGetObjectReference (_transaction, instance.ID);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void GetObjectReference_KnownObject_Invalid_Throws ()
    {
      var instance = DomainObjectMother.CreateObjectInTransaction<Order> (_transaction);
      LifetimeService.DeleteObject (_transaction, instance);
      Assert.That (instance.TransactionContext[_transaction].IsInvalid, Is.True);

      ClientTransactionTestHelper.CallGetObjectReference (_transaction, instance.ID);
    }

    [Test]
    public void GetObjectReference_UnknownObject_ReturnsUnloadedObject ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvent (
        _transaction,
        mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
      
      var result = ClientTransactionTestHelper.CallGetObjectReference (_transaction, DomainObjectIDs.Order1);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.InstanceOf (typeof (Order)));
      Assert.That (InterceptedDomainObjectCreator.Instance.Factory.WasCreatedByFactory (((object) result).GetType()), Is.True);
      Assert.That (result.ID, Is.EqualTo (DomainObjectIDs.Order1));
      Assert.That (_transaction.IsEnlisted (result), Is.True);
      Assert.That (result.TransactionContext[_transaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void GetObjectReference_UnknownObject_BindsToBindingClientTransactionOnly ()
    {
      var unboundResult = ClientTransactionTestHelper.CallGetObjectReference (_transaction, DomainObjectIDs.Order1);
      Assert.That (unboundResult.HasBindingTransaction, Is.False);

      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var boundResult = ClientTransactionTestHelper.CallGetObjectReference (bindingTransaction, DomainObjectIDs.Order1);
      Assert.That (boundResult.HasBindingTransaction, Is.True);
      Assert.That (boundResult.GetBindingTransaction(), Is.SameAs (bindingTransaction));
    }

    [Test]
    public void GetInvalidObjectReference ()
    {
      var invalidObject = DomainObjectMother.CreateObjectInTransaction<Order> (_transaction);
      _transaction.Execute (invalidObject.Delete);

      Assert.That (invalidObject.TransactionContext[_transaction].State, Is.EqualTo (StateType.Invalid));

      var invalidObjectReference = ClientTransactionTestHelper.CallGetInvalidObjectReference (_transaction, invalidObject.ID);

      Assert.That (invalidObjectReference, Is.SameAs (invalidObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has "
        + "not been marked invalid.\r\nParameter name: id")]
    public void GetInvalidObjectReference_ThrowsWhenNotInvalid ()
    {
      ClientTransactionTestHelper.CallGetInvalidObjectReference (_transaction, DomainObjectIDs.Order1);
    }

    [Test]
    public void IsInvalid ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order> (_transaction);
      Assert.That (_transaction.IsInvalid (domainObject.ID), Is.False);

      _transaction.Execute (domainObject.Delete);

      Assert.That (_transaction.IsInvalid (domainObject.ID), Is.True);
    }

    [Test]
    public void EnsureDataAvailable_AlreadyLoaded ()
    {
      var domainObject = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      _transaction.EnsureDataAvailable (domainObject.ID);

      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
    }

    [Test]
    public void EnsureDataAvailable_NotLoadedYet ()
    {
      var domainObject = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      _transaction.EnlistDomainObject (domainObject);
      Assert.That (_dataManager.DataContainers[domainObject.ID], Is.Null);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      _transaction.EnsureDataAvailable (domainObject.ID);

      listenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
          Arg.Is (_transaction), 
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { DomainObjectIDs.Order1 })));
      listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (
          Arg.Is (_transaction),
          Arg<ReadOnlyCollection<DomainObject>>.List.ContainsAll (new[] { domainObject })));
      Assert.That (_dataManager.DataContainers[domainObject.ID], Is.Not.Null);
      Assert.That (_dataManager.DataContainers[domainObject.ID].DomainObject, Is.SameAs (domainObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void EnsureDataAvailable_Invalid ()
    {
      Order domainObject = _transaction.Execute (() =>Order.NewObject ());
      _transaction.Execute (domainObject.Delete);

      _transaction.EnsureDataAvailable (domainObject.ID);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void EnsureDataAvailable_NotFound ()
    {
      var domainObject = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();

      _transaction.EnlistDomainObject (domainObject);
      _transaction.EnsureDataAvailable (domainObject.ID);
    }

    [Test]
    public void EnsureDataAvailable_NotEnlisted ()
    {
      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      _transaction.EnsureDataAvailable (DomainObjectIDs.Order1);

      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
    }

    [Test]
    public void EnsureDataAvailable_Many_AlreadyLoaded ()
    {
      var domainObject1 = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      var domainObject2 = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order2));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      _transaction.EnsureDataAvailable (new[] { domainObject1.ID, domainObject2.ID });

      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg<ClientTransaction>.Is.Anything, Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
    }

    [Test]
    public void EnsureDataAvailable_Many_NotLoadedYet ()
    {
      var domainObject1 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);

      _transaction.EnlistDomainObject (domainObject1);
      _transaction.EnlistDomainObject (domainObject2);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      _transaction.EnsureDataAvailable (new[] { domainObject1.ID, domainObject2.ID });

      listenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
          Arg.Is (_transaction), 
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 })));
    }

    [Test]
    public void EnsureDataAvailable_Many_SomeLoadedSomeNot ()
    {
      var domainObject1 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);
      var domainObject3 = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order3));

      _transaction.EnlistDomainObject (domainObject1);
      _transaction.EnlistDomainObject (domainObject2);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      _transaction.EnsureDataAvailable (new[] { domainObject1.ID, domainObject2.ID, domainObject3.ID });

      listenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
          Arg.Is (_transaction),
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 })));
      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll (new[] { DomainObjectIDs.Order3 })));
    }

    [Test]
    public void EnsureDataAvailable_Many_SomeLoadedSomeNot_PerformsBulkLoad ()
    {
      var domainObject1 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order2);
      var domainObject3 = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order3));

      _transaction.EnlistDomainObject (domainObject1);
      _transaction.EnlistDomainObject (domainObject2);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      _transaction.EnsureDataAvailable (new[] { domainObject1.ID, domainObject2.ID, domainObject3.ID });

      listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (
          Arg.Is (_transaction), 
          Arg<ReadOnlyCollection<DomainObject>>.List.ContainsAll (new[] { domainObject1, domainObject2 })));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void EnsureDataAvailable_Many_Invalid ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject ());
      _transaction.Execute (domainObject.Delete);

      _transaction.EnsureDataAvailable (new[] { domainObject.ID });
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException))]
    public void EnsureDataAvailable_Many_NotFound ()
    {
      var domainObject = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();

      _transaction.EnlistDomainObject (domainObject);
      _transaction.EnsureDataAvailable (new[] { domainObject.ID });
    }

    [Test]
    public void EnsureDataAvailable_Many_NotEnlisted ()
    {
      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      _transaction.EnsureDataAvailable (new[] { DomainObjectIDs.Order1 });

      Assert.That (_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Virtual ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      _transaction.EnsureDataComplete (endPointID);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Real ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "Customer");
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Null);

      _transaction.EnsureDataComplete (endPointID);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Complete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _transaction.Execute (() => Customer.GetObject (DomainObjectIDs.Customer1).Orders);

      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);

      _transaction.EnsureDataComplete (endPointID);
      Assert.That (_dataManager.GetRelationEndPointWithoutLoading (endPointID), Is.Not.Null);
    }

    [Test]
    public void EnsureDataComplete_EndPoint_Incomplete ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");
      _transaction.Execute (() => Customer.GetObject (DomainObjectIDs.Customer1).Orders);
      
      var endPoint = (ICollectionEndPoint) _dataManager.GetRelationEndPointWithoutLoading (endPointID);
      Assert.That (endPoint, Is.Not.Null);
      endPoint.MarkDataIncomplete ();
      Assert.That (endPoint.IsDataComplete, Is.False);

      _transaction.EnsureDataComplete (endPointID);
      Assert.That (endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void GetEnlistedDomainObjects ()
    {
      var order1 = _transaction.Execute(() => Order.NewObject ());
      var order2 = _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.GetEnlistedDomainObjects ().ToArray (), Is.EquivalentTo (new[] { order1, order2 }));
    }

    [Test]
    public void EnlistedDomainObjectCount ()
    {
      _transaction.Execute (() => Order.NewObject ());
      _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.EnlistedDomainObjectCount, Is.EqualTo (2));
    }

    [Test]
    public void IsEnlisted ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.IsEnlisted (order), Is.True);
      Assert.That (ClientTransaction.CreateRootTransaction().IsEnlisted (order), Is.False);
    }

    [Test]
    public void GetEnlistedDomainObject ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.GetEnlistedDomainObject (order.ID), Is.SameAs (order));
    }

    [Test]
    public void EnlistDomainObject ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      Assert.That (_transaction.IsEnlisted (order), Is.False);

      _transaction.EnlistDomainObject (order);

      Assert.That (_transaction.IsEnlisted (order), Is.True);
    }

    [Test]
    public void EnlistDomainObject_DoesntLoad ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      Assert.That (_transaction.IsEnlisted (order), Is.False);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      _transaction.EnlistDomainObject (order);

      listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
    }

    [Test]
    public void EnlistDomainObject_InvalidObjects ()
    {
      var invalidObject = _transaction.Execute (() => Order.NewObject ());
      _transaction.Execute (invalidObject.Delete);
      Assert.That (invalidObject.TransactionContext[_transaction].IsInvalid, Is.True);

      var newTransaction = ClientTransaction.CreateRootTransaction();
      newTransaction.EnlistDomainObject (invalidObject);

      Assert.That (newTransaction.IsEnlisted (invalidObject), Is.True);
    }

    [Test]
    public void CopyCollectionEventHandlers ()
    {
      var order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      
      bool orderItemAdded = false;
      _transaction.Execute (() => order.OrderItems.Added += delegate { orderItemAdded = true; });

      var otherTransaction = ClientTransaction.CreateRootTransaction ();
      Assert.That (orderItemAdded, Is.False);

      otherTransaction.EnlistDomainObject (order);
      otherTransaction.CopyCollectionEventHandlers (order, _transaction);

      otherTransaction.Execute (() => order.OrderItems.Add (OrderItem.NewObject ()));
      Assert.That (orderItemAdded, Is.True);
    }

    [Test]
    public void CreateSubTransaction_WithDefaultComponentFactory ()
    {
      Assert.That (_transaction.IsReadOnly, Is.False);
      
      var subTransaction = _transaction.CreateSubTransaction ();
      Assert.That (subTransaction, Is.TypeOf (typeof (ClientTransaction)));
      Assert.That (subTransaction.ParentTransaction, Is.SameAs (_transaction));
      Assert.That (_transaction.IsReadOnly, Is.True);
      Assert.That (_transaction.SubTransaction, Is.SameAs (subTransaction));

      Assert.That (subTransaction.Extensions, Is.SameAs (_transaction.Extensions));
      Assert.That (subTransaction.ApplicationData, Is.SameAs (_transaction.ApplicationData));
      
      var enlistedObjectManager = ClientTransactionTestHelper.GetEnlistedDomainObjectManager (subTransaction);
      Assert.That (enlistedObjectManager, Is.TypeOf (typeof (DelegatingEnlistedDomainObjectManager)));
      Assert.That (((DelegatingEnlistedDomainObjectManager) enlistedObjectManager).TargetTransaction, Is.SameAs (_transaction));

      var invalidDomainObjectManager = ClientTransactionTestHelper.GetInvalidDomainObjectManager (subTransaction);
      Assert.That (invalidDomainObjectManager, Is.TypeOf (typeof (SubInvalidDomainObjectManager)));
      Assert.That (((SubInvalidDomainObjectManager) invalidDomainObjectManager).ParentTransactionManager, 
          Is.SameAs (ClientTransactionTestHelper.GetInvalidDomainObjectManager (_transaction)));

      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction);
      Assert.That (persistenceStrategy, Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_WithCustomFactory ()
    {
      Assert.That (_transaction.IsReadOnly, Is.False);

      var fakePersistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      fakePersistenceStrategy.Stub (stub => stub.ParentTransaction).Return (_transaction);
      var fakeSubTransaction = ClientTransactionObjectMother.CreateTransactionWithPersistenceStrategy<ClientTransaction> (fakePersistenceStrategy);

      Func<ClientTransaction, IInvalidDomainObjectManager, ClientTransaction> factoryMock = (tx, invalidDomainObjectManager) =>
      {
        Assert.That (tx, Is.SameAs (_transaction));
        Assert.That (invalidDomainObjectManager, Is.SameAs (ClientTransactionTestHelper.GetInvalidDomainObjectManager (_transaction)));
        return fakeSubTransaction;
      };

      var subTransaction = _transaction.CreateSubTransaction (factoryMock);

      Assert.That (subTransaction, Is.SameAs (fakeSubTransaction));
      Assert.That (_transaction.SubTransaction, Is.SameAs (subTransaction));
    }

    [Test]
    public void CreateSubTransaction_Events ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);
      var mockRepository = listenerMock.GetMockRepository();
      var componentFactoryPartialMock = mockRepository.PartialMock<SubClientTransactionComponentFactory> (
          _transaction, 
          ClientTransactionTestHelper.GetInvalidDomainObjectManager (_transaction));
      var eventReceiverMock = mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_transaction);
      
      var fakePersistenceStrategy = MockRepository.GenerateStub<IPersistenceStrategy> ();
      fakePersistenceStrategy.Stub (stub => stub.ParentTransaction).Return (_transaction);

      using (mockRepository.Ordered ())
      {
        listenerMock
            .Expect (mock => mock.SubTransactionCreating (_transaction))
            .WhenCalled (mi => Assert.That (_transaction.IsReadOnly, Is.False));
        componentFactoryPartialMock
            .Expect (mock => mock.CreatePersistenceStrategy (Arg<Guid>.Is.Anything))
            .Return (fakePersistenceStrategy);
        eventReceiverMock
            .Expect (mock => mock.SubTransactionCreated (
                Arg.Is (_transaction), 
                Arg<SubTransactionCreatedEventArgs>.Matches (arg => arg.SubTransaction.ParentTransaction == _transaction)))
            .WhenCalled (mi =>
            {
              Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
              Assert.That (_transaction.IsReadOnly, Is.True);
            });
        listenerMock
            .Expect (mock => mock.SubTransactionCreated (
                Arg.Is (_transaction), 
                Arg<ClientTransaction>.Matches (subTx => subTx.ParentTransaction == _transaction)));
      }

      mockRepository.ReplayAll ();

      _transaction.CreateSubTransaction ((tx, invalidDomainObjectManager) => ClientTransactionObjectMother.Create<ClientTransaction> (componentFactoryPartialMock));
      mockRepository.VerifyAll ();
    }

    [Test]
    public void CreateSubTransaction_CancellationInCreatingNotification ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);
      var eventReceiverMock = MockRepository.GenerateStrictMock<ClientTransactionMockEventReceiver> (_transaction);
      Func<ClientTransaction, IInvalidDomainObjectManager, ClientTransaction> factoryMock = (tx, invalidDomainObjectManager) =>
      {
        Assert.Fail ("Must not be called.");
        return null;
      };

      var exception = new InvalidOperationException ("Canceled");
      listenerMock
          .Expect (mock => mock.SubTransactionCreating (_transaction))
          .Throw (exception);
      listenerMock.Replay ();
      eventReceiverMock.Replay ();

      try
      {
        _transaction.CreateSubTransaction (factoryMock);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }

      listenerMock.AssertWasNotCalled (mock => mock.SubTransactionCreated (Arg<ClientTransaction>.Is.Anything, Arg<ClientTransaction>.Is.Anything));

      eventReceiverMock.AssertWasNotCalled (mock => mock.SubTransactionCreated (Arg<object>.Is.Anything, Arg<SubTransactionCreatedEventArgs>.Is.Anything));

      Assert.That (_transaction.IsReadOnly, Is.False);
    }

    [Test]
    public void CreateSubTransaction_ExceptionInFactory ()
    {
      var exception = new InvalidOperationException ("Canceled");
      Func<ClientTransaction, IInvalidDomainObjectManager, ClientTransaction> factoryMock = (tx, invalidDomainObjectManager) =>
      {
        throw (exception);
      };

      try
      {
        _transaction.CreateSubTransaction (factoryMock);
        Assert.Fail ("Expected exception");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }

      Assert.That (_transaction.IsReadOnly, Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The given component factory did not create a sub-transaction for this transaction.")]
    public void CreateSubTransaction_Throws_WhenParentTransactionDoesNotMatch ()
    {
      try
      {
        _transaction.CreateSubTransaction ((tx, invalidDomainObjectManager) => ClientTransaction.CreateRootTransaction());
      }
      catch (InvalidOperationException)
      {
        Assert.That (_transaction.IsReadOnly, Is.False);
        throw;
      }
    }

    [Test]
    public void OnSubTransactionCreated_WithCustomFactory ()
    {
      var fakeSubTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransactionTestHelper.SetIsReadOnly(_transactionWithMocks, true);

      PrivateInvoke.InvokeNonPublicMethod (_transactionWithMocks, "OnSubTransactionCreated", new SubTransactionCreatedEventArgs (fakeSubTransaction));

      Assert.That (_transactionWithMocks.SubTransaction, Is.SameAs (fakeSubTransaction));
    }

    [Test]
    public void Discard ()
    {
      var listenerMock = _mockRepository.StrictMock<IClientTransactionListener>();
      listenerMock.Expect (mock => mock.TransactionDiscarding (_transactionWithMocks));

      _persistenceStrategyMock.Stub (stub => stub.ParentTransaction).Return (null);

      _mockRepository.ReplayAll();
      _fakeListeners.AddListener (listenerMock);

      Assert.That (_transactionWithMocks.IsDiscarded, Is.False);
      var eventSink = ClientTransactionTestHelper.GetTransactionEventSink (_transactionWithMocks);
      Assert.That (eventSink.Listeners.OfType<InvalidatedTransactionListener> ().SingleOrDefault (), Is.Null);

      _transactionWithMocks.Discard();

      _mockRepository.VerifyAll();
      Assert.That (_transactionWithMocks.IsDiscarded, Is.True);
      Assert.That (eventSink.Listeners.OfType<InvalidatedTransactionListener>().SingleOrDefault(), Is.Not.Null);
    }

    [Test]
    public void Discard_WithParentTransaction ()
    {
      var parentTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransactionTestHelper.SetIsReadOnly (parentTransaction, true);
      ClientTransactionTestHelper.SetActiveSubTransaction (parentTransaction, _transactionWithMocks);

      _persistenceStrategyMock.Stub (stub => stub.ParentTransaction).Return (parentTransaction);

      _mockRepository.ReplayAll ();

      Assert.That (parentTransaction.IsReadOnly, Is.True);
      Assert.That (parentTransaction.SubTransaction, Is.Not.Null);

      _transactionWithMocks.Discard ();

      Assert.That (parentTransaction.IsReadOnly, Is.False);
      Assert.That (parentTransaction.SubTransaction, Is.Null);
    }

    [Test]
    public void Discard_Twice ()
    {
      var parentTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransactionTestHelper.SetIsReadOnly (parentTransaction, true);
      ClientTransactionTestHelper.SetActiveSubTransaction (parentTransaction, _transactionWithMocks);

      _persistenceStrategyMock.Stub (stub => stub.ParentTransaction).Return (parentTransaction);
      _mockRepository.ReplayAll();

      _transactionWithMocks.Discard();

      var otherSubTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransactionTestHelper.SetIsReadOnly (parentTransaction, true);
      ClientTransactionTestHelper.SetActiveSubTransaction (parentTransaction, otherSubTransaction);

      var listenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      listenerMock.Replay();
      _fakeListeners.AddListener (listenerMock);

      _transactionWithMocks.Discard();

      listenerMock.AssertWasNotCalled (mock => mock.TransactionDiscarding (_transactionWithMocks));
      Assert.That (parentTransaction.IsReadOnly, Is.True);
      Assert.That (parentTransaction.SubTransaction, Is.SameAs (otherSubTransaction));
    }
  }
}