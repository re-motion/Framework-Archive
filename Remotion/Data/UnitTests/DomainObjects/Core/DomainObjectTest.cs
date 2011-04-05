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
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class DomainObjectTest : StandardMappingTest
  {
    private ClientTransactionMock _transaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransactionMock ();
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject ());
      Assert.That (domainObject.OnReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing_InRightTransaction ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject ());
      Assert.That (domainObject.OnReferenceInitializingTx, Is.SameAs (_transaction));
    }

    [Test]
    public void Ctor_RaisesReferenceInitializing_CalledBeforeCtor ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject ());
      Assert.That (domainObject.OnReferenceInitializingCalledBeforeCtor, Is.True);
    }

    [Test]
    public void Ctor_RaisesNewObjectCreating ()
    {
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (_transaction);

      var instance = _transaction.Execute (() => Order.NewObject ());

      listenerMock.AssertWasCalled (mock => mock.NewObjectCreating (_transaction, typeof (Order), instance));
    }

    [Test]
    public void Ctor_CreatesObjectID ()
    {
      var instance = _transaction.Execute (() => Order.NewObject ());

      Assert.That (instance.ID, Is.Not.Null);
      Assert.That (instance.ID.ClassDefinition, Is.SameAs (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order))));
    }

    [Test]
    public void Ctor_Binding ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var boundInstance = bindingTransaction.Execute (() => Order.NewObject ());

      Assert.That (boundInstance.HasBindingTransaction, Is.True);
      Assert.That (boundInstance.GetBindingTransaction(), Is.SameAs (bindingTransaction));

      var nonBindingTransaction = ClientTransaction.CreateRootTransaction ();
      var nonBoundInstance = nonBindingTransaction.Execute (() => Order.NewObject ());

      Assert.That (nonBoundInstance.HasBindingTransaction, Is.False);
    }

    [Test]
    public void Ctor_CreatesAndRegistersDataContainer ()
    {
      var instance = _transaction.Execute (() => Order.NewObject ());

      var dataContainer = _transaction.DataManager.DataContainerMap[instance.ID];
      Assert.That (dataContainer, Is.Not.Null);
      Assert.That (dataContainer.DomainObject, Is.SameAs (instance));
      Assert.That (dataContainer.ClientTransaction, Is.SameAs (_transaction));
    }

    [Test]
    public void Ctor_EnlistsObjectInTransaction ()
    {
      var instance = _transaction.Execute (() => Order.NewObject ());

      Assert.That (_transaction.IsEnlisted (instance), Is.True);
    }

    [Test]
    public void Ctor_WithVirtualPropertyCall_Allowed ()
    {
      var orderItem = _transaction.Execute (() => OrderItem.NewObject ("Test Toast"));
      Assert.That (_transaction.Execute (() => orderItem.Product), Is.EqualTo ("Test Toast"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    public void Initialize_ThrowsForNewObject ()
    {
      var orderItem = _transaction.Execute (() => OrderItem.NewObject ("Test Toast"));
      orderItem.Initialize (DomainObjectIDs.OrderItem1, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    public void Initialize_ThrowsForLoadedObject ()
    {
      var orderItem = _transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      orderItem.Initialize (DomainObjectIDs.OrderItem1, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The object cannot be initialized, it already has an ID.")]
    public void Initialize_ThrowsForDeserializedObject ()
    {
      var orderItem = _transaction.Execute (() => OrderItem.GetObject (DomainObjectIDs.OrderItem1));
      var deserializedOrderItem = Serializer.SerializeAndDeserialize (orderItem);
      deserializedOrderItem.Initialize (DomainObjectIDs.OrderItem1, null);
    }

    [Test]
    public void Initialize_WithUninitializedObject_SetsID ()
    {
      var type = InterceptedDomainObjectCreator.Instance.Factory.GetConcreteDomainObjectType (typeof (OrderItem));
      var orderItem = (OrderItem) FormatterServices.GetSafeUninitializedObject (type);
      orderItem.Initialize (DomainObjectIDs.OrderItem1, ClientTransaction.Current);

      Assert.That (orderItem.ID, Is.EqualTo (DomainObjectIDs.OrderItem1));
    }

    [Test]
    public void HasBindingTransaction_BoundObject ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      
      var obj = bindingTransaction.Execute (() => Order.NewObject ());
      Assert.That (obj.HasBindingTransaction, Is.True);
    }

    [Test]
    public void HasBindingTransaction_UnboundObject ()
    {
      var obj = _transaction.Execute (() => Order.NewObject ());
      Assert.That (obj.HasBindingTransaction, Is.False);
    }

    [Test]
    public void GetBindingTransaction_BoundObject ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var obj = bindingTransaction.Execute (() => Order.NewObject ());
      Assert.That (obj.GetBindingTransaction(), Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This object has not been bound to a specific transaction, it "
        + "uses the current transaction when it is accessed. Use HasBindingTransaction to check whether an object has been bound or not.")]
    public void GetBindingTransaction_UnboundObject ()
    {
      var obj = _transaction.Execute (() => Order.NewObject ());
      Dev.Null = obj.GetBindingTransaction();
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_IDAndBindingTransaction ()
    {
      var domainObject = _transaction.Execute (() => Order.NewObject()); // indirect call of RaiseReferenceInitializatingEvent
      Assert.That (domainObject.OnReferenceInitializingCalled, Is.True);

      Assert.That (domainObject.OnReferenceInitializingID, Is.EqualTo (domainObject.ID));
      Assert.That (domainObject.OnReferenceInitializingBindingTransaction, Is.Null);

      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var boundDomainObject = (Order) InterceptedDomainObjectCreator.Instance.CreateObjectReference (DomainObjectIDs.Order1, bindingTransaction);
      Assert.That (boundDomainObject.OnReferenceInitializingBindingTransaction, Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_PropertyAccessForbidden ()
    {
      _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o.OrderNumber));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_PropertiesForbidden ()
    {
      _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o.Properties));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_CurrentPropertyForbidden ()
    {
      _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o.CurrentProperty));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_TransactionContextIsRestricted ()
    {
      var result = _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o.DefaultTransactionContext));
      Assert.That (result, Is.TypeOf (typeof (InitializedEventDomainObjectTransactionContextDecorator)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_CallsReferenceInitializing_DeleteForbidden ()
    {
      _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => { o.Delete (); return o; }));
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_InvokesMixinHook ()
    {
      var domainObject = _transaction.Execute (() => HookedTargetClass.NewObject()); // indirect call of RaiseReferenceInitializatingEvent
      var mixinInstance = Mixin.Get<HookedDomainObjectMixin> (domainObject);

      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "While the OnReferenceInitializing event is executing, this member cannot be used.")]
    public void RaiseReferenceInitializatingEvent_InvokesMixinHook_WhilePropertyAccessForbidden ()
    {
      var mixinInstance = new HookedDomainObjectMixin ();
      mixinInstance.InitializationHandler += (sender, args) => Dev.Null = ((HookedDomainObjectMixin) sender).Target.Property;

      using (new MixedObjectInstantiationScope (mixinInstance))
      {
        _transaction.Execute (() => HookedTargetClass.NewObject ()); // indirect call of RaiseReferenceInitializatingEvent
      }
    }

    [Test]
    public void RaiseReferenceInitializatingEvent_ResetsFlagAfterNotification ()
    {
      var order = _transaction.Execute (() => DomainObjectTestHelper.ExecuteInReferenceInitializing_NewObject (o => o));
      Dev.Null = _transaction.Execute (() => order.OrderNumber); // succeeds
    }

    [Test]
    public void DefaultTransactionContext_Current ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (_transaction.Execute (() => order.DefaultTransactionContext.ClientTransaction), Is.SameAs (_transaction));
    }

    [Test]
    public void DefaultTransactionContext_Bound ()
    {
      var bindingTransaction = ClientTransaction.CreateBindingTransaction ();
      var order = bindingTransaction.Execute (() => Order.NewObject ());
      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (bindingTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "No ClientTransaction has been associated with the current thread or this object.")]
    public void DefaultTransactionContext_Null ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Dev.Null = order.DefaultTransactionContext;
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = 
        @"Cannot delete DomainObject 'Order|.*|System\.Guid', because it belongs to a different ClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void Delete_ChecksTransaction ()
    {
      var order = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      Assert.That (_transaction.IsEnlisted (order), Is.False);
      _transaction.Execute (order.Delete);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage = "Domain object '.*' cannot be used in the given transaction "
        + "as it was loaded or created in another transaction. Enter a scope for the transaction, or enlist the object in "
        + "the transaction. \\(If no transaction was explicitly given, ClientTransaction.Current was used.\\)", MatchType = MessageMatch.Regex)]
    public void PropertyAccess_ThrowsWhenNotEnlisted ()
    {
      Order order = _transaction.Execute (() => Order.NewObject ());
      var otherTransaction = ClientTransaction.CreateRootTransaction ();
      Assert.That (otherTransaction.IsEnlisted (order), Is.False);
      Dev.Null = otherTransaction.Execute (() => order.OrderNumber);
    }

    [Test]
    public void OnLoaded_CanAccessPropertyValues ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      order.ProtectedLoaded += ((sender, e) => Assert.That (((Order) sender).OrderNumber, Is.EqualTo (1)));

      newTransaction.EnlistDomainObject (order);
      order.TransactionContext[newTransaction].EnsureDataAvailable ();
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      var order = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);
      _transaction.EnlistDomainObject (order);
      Assert.That (_transaction.DataManager.DataContainerMap[order.ID], Is.Null);
      
      _transaction.Execute (order.EnsureDataAvailable);

      Assert.That (_transaction.DataManager.DataContainerMap[order.ID], Is.Not.Null);
      Assert.That (_transaction.DataManager.DataContainerMap[order.ID].DomainObject, Is.SameAs (order));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "DomainObject.GetType should not be used.")]
    public void GetType_Throws ()
    {
      try
      {
        Order order = _transaction.Execute (() => Order.NewObject ());
        typeof (DomainObject).GetMethod ("GetType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Invoke (
            order, new object[0]);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void GetPublicDomainObjectType ()
    {
      Customer customer = _transaction.Execute (() => Customer.NewObject ());
      Assert.That (customer.GetPublicDomainObjectType(), Is.SameAs (typeof (Customer)));
    }

    [Test]
    public new void ToString ()
    {
      Order order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (order.ToString (), Is.EqualTo (order.ID.ToString ()));
    }

    [Test]
    public void State ()
    {
      Customer customer = _transaction.Execute (() => Customer.GetObject (DomainObjectIDs.Customer1));

      _transaction.Execute (() => Assert.That (customer.State, Is.EqualTo (StateType.Unchanged)));
      _transaction.Execute (() => customer.Name = "New name");
      _transaction.Execute (() => Assert.That (customer.State, Is.EqualTo (StateType.Changed)));
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _transaction.Execute (() => Assert.That (order.State, Is.EqualTo (StateType.Unchanged)));

      _transaction.Execute (order.MarkAsChanged);
      
      _transaction.Execute (() => Assert.That (order.State, Is.EqualTo (StateType.Changed)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DomainObjects can be marked as changed.")]
    public void MarkAsChangedThrowsWhenNew ()
    {
      Order order = _transaction.Execute (() => Order.NewObject ());
      _transaction.Execute (order.MarkAsChanged);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DomainObjects can be marked as changed.")]
    public void MarkAsChangedThrowsWhenDeleted ()
    {
      Order order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      _transaction.Execute (order.Delete);
      _transaction.Execute (order.MarkAsChanged);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void NetObject_WithoutTransaction ()
    {
      Order.NewObject ();
    }

    [Test]
    public void NewObject_CallsCtor ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (order.CtorCalled, Is.True);
    }

    [Test]
    public void NewObject_ProtectedConstructor ()
    {
      Dev.Null = _transaction.Execute (() => Company.NewObject ());
    }

    [Test]
    public void NewObject_PublicConstructor ()
    {
      Dev.Null = _transaction.Execute (() => Customer.NewObject ());
    }

    [Test]
    public void NewObject_SetsNeedsLoadModeDataContainerOnly_True ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void GetObject_SetsNeedsLoadModeDataContainerOnly_True_ ()
    {
      var order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No ClientTransaction has been associated with the current thread.")]
    public void GetObject_WithoutTransaction ()
    {
      Order.GetObject (DomainObjectIDs.Order1);
    }

    [Test]
    public void GetObject_Deleted ()
    {
      var order = _transaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));

      _transaction.Execute (order.Delete);

      _transaction.Execute (() => Assert.That (Order.GetObject (DomainObjectIDs.Order1, true), Is.SameAs (order)));
      _transaction.Execute (() => Assert.That (order.State, Is.EqualTo (StateType.Deleted)));
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_False_BeforeGetObject ()
    {
      var creator = DomainObjectIDs.Order1.ClassDefinition.GetDomainObjectCreator ();
      var order = (Order) creator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_True_AfterOnLoaded ()
    {
      var creator = DomainObjectIDs.Order1.ClassDefinition.GetDomainObjectCreator ();
      var order = (Order) creator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);

      PrivateInvoke.InvokeNonPublicMethod (order, typeof (DomainObject), "OnLoaded");

      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_Serialization_True ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.True);

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      Assert.That (deserializedOrder.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_Serialization_False ()
    {
      var creator = DomainObjectIDs.Order1.ClassDefinition.GetDomainObjectCreator ();
      var order = (Order) creator.CreateObjectReference (DomainObjectIDs.Order1, _transaction);

      Assert.That (order.NeedsLoadModeDataContainerOnly, Is.False);

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      Assert.That (deserializedOrder.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_Serialization_ISerializable_True ()
    {
      var classWithAllDataTypes = _transaction.Execute (() => ClassWithAllDataTypes.NewObject ());
      Assert.That (classWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.True);

      var deserializedClassWithAllDataTypes = Serializer.SerializeAndDeserialize (classWithAllDataTypes);
      Assert.That (deserializedClassWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.True);
    }

    [Test]
    public void NeedsLoadModeDataContainerOnly_Serialization_ISerializable_False ()
    {
      var creator = DomainObjectIDs.Order1.ClassDefinition.GetDomainObjectCreator ();
      var classWithAllDataTypes = (ClassWithAllDataTypes) creator.CreateObjectReference (DomainObjectIDs.ClassWithAllDataTypes1, _transaction);

      Assert.That (classWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.False);

      var deserializedClassWithAllDataTypes = Serializer.SerializeAndDeserialize (classWithAllDataTypes);
      Assert.That (deserializedClassWithAllDataTypes.NeedsLoadModeDataContainerOnly, Is.False);
    }

    [Test]
    public void Properties ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      var propertyIndexer = _transaction.Execute (() => order.Properties);
      Assert.That (propertyIndexer, Is.Not.Null);
      Assert.That (propertyIndexer.DomainObject, Is.SameAs (order));

      var propertyIndexer2 = _transaction.Execute (() => order.Properties);
      Assert.That (propertyIndexer, Is.SameAs (propertyIndexer2));
    }

    [Test]
    public void Properties_Serialization ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      var propertyIndexer = _transaction.Execute (() => order.Properties);
      Assert.That (propertyIndexer, Is.Not.Null);
      Assert.That (propertyIndexer.DomainObject, Is.SameAs (order));

      var deserializedOrder = Serializer.SerializeAndDeserialize (order);
      var newPropertyIndexer = _transaction.Execute (() => deserializedOrder.Properties);
      Assert.That (newPropertyIndexer, Is.Not.Null);
      Assert.That (newPropertyIndexer.DomainObject, Is.SameAs (deserializedOrder));
    }

    [Test]
    public void TransactionContext ()
    {
      var order = _transaction.Execute (() => Order.NewObject ());
      var transactionContextIndexer = order.TransactionContext;

      Assert.That (transactionContextIndexer, Is.InstanceOf (typeof (DomainObjectTransactionContextIndexer)));
      Assert.That (((DomainObjectTransactionContext) transactionContextIndexer[_transaction]).DomainObject, Is.SameAs (order));
    }
  }
}
