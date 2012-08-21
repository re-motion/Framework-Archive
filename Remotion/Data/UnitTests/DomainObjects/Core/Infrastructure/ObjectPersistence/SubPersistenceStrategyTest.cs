// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class SubPersistenceStrategyTest : ClientTransactionBaseTest
  {
    private IParentTransactionContext _parentTransactionContextMock;
    private IUnlockedParentTransactionContext _unlockedParentTransactionContextMock;
    private SubPersistenceStrategy _persistenceStrategy;

    private IQuery _queryStub;
    
    private PropertyDefinition _orderNumberPropertyDefinition;
    private PropertyDefinition _fileNamePropertyDefinition;
    private PropertyDefinition _productPropertyDefinition;

    private RelationEndPointID _virtualObjectRelationEndPointID;
    private RelationEndPointID _collectionEndPointID;
    private RelationEndPointID _nonVirtualEndPointID;

    private ILoadedObjectDataProvider _alreadyLoadedObjectDataProviderMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransactionContextMock = MockRepository.GenerateStrictMock<IParentTransactionContext> ();
      _unlockedParentTransactionContextMock = MockRepository.GenerateStrictMock<IUnlockedParentTransactionContext> ();
      _persistenceStrategy = new SubPersistenceStrategy (_parentTransactionContextMock);

      _queryStub = MockRepository.GenerateStub<IQuery>();
    
      _orderNumberPropertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");
      _fileNamePropertyDefinition = GetPropertyDefinition (typeof (OrderTicket), "FileName");
      _productPropertyDefinition = GetPropertyDefinition (typeof (OrderItem), "Product");

      _virtualObjectRelationEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetEndPointDefinition (typeof (Order), "OrderTicket"));
      _collectionEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetEndPointDefinition (typeof (Order), "OrderItems"));
      _nonVirtualEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, GetEndPointDefinition (typeof (Order), "Customer"));
      
      _alreadyLoadedObjectDataProviderMock = MockRepository.GenerateStrictMock<ILoadedObjectDataProvider>();
    }

    [Test]
    public void CreateNewObjectID ()
    {
      var classDefinition = GetTypeDefinition (typeof (Order));
      var fakeResult = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Expect (mock => mock.CreateNewObjectID (classDefinition))
          .Return (fakeResult);

      var result = _persistenceStrategy.CreateNewObjectID (classDefinition);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void LoadObjectData_Single ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> (objectID);
      var parentDataContainer = CreateChangedDataContainer (objectID, 4711, _orderNumberPropertyDefinition, 17);
      CheckDataContainer (parentDataContainer, objectID, 4711, StateType.Changed, _orderNumberPropertyDefinition, 17, 0, true);

      var parentEventListenerMock = MockRepository.GenerateStrictMock<IDataContainerEventListener>();
      parentDataContainer.SetEventListener (parentEventListenerMock);

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Return (parentDataContainer);

      var result = _persistenceStrategy.LoadObjectData (objectID);

      _parentTransactionContextMock.VerifyAllExpectations();
      parentEventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueReading (Arg<DataContainer>.Is.Anything, Arg<PropertyDefinition>.Is.Anything, Arg<ValueAccess>.Is.Anything));

      Assert.That (result, Is.TypeOf<FreshlyLoadedObjectData> ());
      Assert.That (result.ObjectID, Is.EqualTo (objectID));
      var dataContainer = ((FreshlyLoadedObjectData) result).FreshlyLoadedDataContainer;
      CheckDataContainer (dataContainer, objectID, 4711, StateType.Unchanged, _orderNumberPropertyDefinition, 17, 17, false);
    }

    [Test]
    public void LoadObjectData_Single_PropagatesExceptionsFromGetDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> (objectID);
      var exception = new Exception ("E.g., object is invalid.");

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Throw (exception);

      Assert.That (() => _persistenceStrategy.LoadObjectData (objectID), Throws.Exception.SameAs (exception));

      _parentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadObjectData_Single_ParentObjectIsDeleted ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> (objectID);
      var deletedParentDataContainer = DataContainerObjectMother.CreateDeleted (objectID);

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Return (deletedParentDataContainer);

      Assert.That (
          () => _persistenceStrategy.LoadObjectData (objectID),
          Throws.TypeOf<ObjectDeletedException> ().With.Message.EqualTo (
            "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is already deleted in the parent transaction."));

      _parentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadObjectData_Single_NoParentObjectFound ()
    {
      var objectID = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (null);

      var result = _persistenceStrategy.LoadObjectData (objectID);

      _parentTransactionContextMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<NotFoundLoadedObjectData>());
      Assert.That (result.ObjectID, Is.EqualTo (objectID));
    }

    [Test]
    public void LoadObjectData_Multiple ()
    {
      var objectID1 = DomainObjectIDs.Order1;
      var objectID2 = DomainObjectIDs.Order2;
      var objectID3 = DomainObjectIDs.Order3;

      var parentObject1 = DomainObjectMother.CreateFakeObject<Order> (objectID1);
      var parentDataContainer1 = CreateChangedDataContainer (objectID1, 4711, _orderNumberPropertyDefinition, 17);
      CheckDataContainer (parentDataContainer1, objectID1, 4711, StateType.Changed, _orderNumberPropertyDefinition, 17, 0, true);

      var parentObject3 = DomainObjectMother.CreateFakeObject<Order> (objectID3);
      var parentDataContainer3 = DataContainerObjectMother.CreateExisting (objectID3);

      var parentEventListenerMock = MockRepository.GenerateStrictMock<IDataContainerEventListener> ();
      parentDataContainer1.SetEventListener (parentEventListenerMock);
      parentDataContainer3.SetEventListener (parentEventListenerMock);

      // Use a strict mock because the parameter should not be enumerated, it should only be passed on TryGetObjects

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObjects (new[] { objectID1, objectID2, objectID3 }))
          .Return (new[] { parentObject1, null, parentObject3 });
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID1, true))
          .Return (parentDataContainer1);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID3, true))
          .Return (parentDataContainer3);

      var result = _persistenceStrategy.LoadObjectData (new[] { objectID1, objectID2, objectID3 }.AsOneTime()).ToList();

      _parentTransactionContextMock.VerifyAllExpectations ();
      parentEventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueReading (Arg<DataContainer>.Is.Anything, Arg<PropertyDefinition>.Is.Anything, Arg<ValueAccess>.Is.Anything));

      Assert.That (result[0], Is.TypeOf<FreshlyLoadedObjectData> ());
      Assert.That (result[0].ObjectID, Is.EqualTo (objectID1));
      CheckDataContainer (((FreshlyLoadedObjectData) result[0]).FreshlyLoadedDataContainer, objectID1, 4711, StateType.Unchanged, _orderNumberPropertyDefinition, 17, 17, false);

      Assert.That (result[1], Is.TypeOf<NotFoundLoadedObjectData> ());
      Assert.That (result[1].ObjectID, Is.EqualTo (objectID2));

      Assert.That (result[2], Is.TypeOf<FreshlyLoadedObjectData> ());
      Assert.That (result[2].ObjectID, Is.EqualTo (objectID3));
    }

    [Test]
    public void LoadObjectData_Multiple_PropagatesExceptionsFromGetDataContainer ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> ();
      var exception = new Exception ("E.g., object is invalid.");

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Throw (exception);

      Assert.That (() => _persistenceStrategy.LoadObjectData (objectID), Throws.Exception.SameAs (exception));

      _parentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadObjectData_Multiple_ParentObjectIsDeleted ()
    {
      var objectID = DomainObjectIDs.Order1;
      var parentObject = DomainObjectMother.CreateFakeObject<Order> ();
      var deletedParentDataContainer = DataContainerObjectMother.CreateDeleted (objectID);

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (parentObject);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (objectID, true))
          .Return (deletedParentDataContainer);

      Assert.That (
          () => _persistenceStrategy.LoadObjectData (objectID),
          Throws.TypeOf<ObjectDeletedException> ().With.Message.EqualTo (
            "Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is already deleted in the parent transaction."));

      _parentTransactionContextMock.VerifyAllExpectations ();
    }

    [Test]
    public void LoadObjectData_Multiple_NoParentObjectFound ()
    {
      var objectID = DomainObjectIDs.Order1;

      _parentTransactionContextMock
          .Expect (mock => mock.TryGetObject (objectID))
          .Return (null);

      var result = _persistenceStrategy.LoadObjectData (objectID);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<NotFoundLoadedObjectData> ());
      Assert.That (result.ObjectID, Is.EqualTo (objectID));
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToAlreadyLoadedObject ()
    {
      var parentObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      _parentTransactionContextMock
          .Expect (mock => mock.ResolveRelatedObject (_virtualObjectRelationEndPointID))
          .Return (parentObject);

      var alreadyLoadedObjectData = MockRepository.GenerateStub<ILoadedObjectData>();
      _alreadyLoadedObjectDataProviderMock
          .Expect (mock => mock.GetLoadedObject (parentObject.ID))
          .Return (alreadyLoadedObjectData);
      
      var result = _persistenceStrategy.ResolveObjectRelationData (_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock);

      _parentTransactionContextMock.VerifyAllExpectations ();
      _alreadyLoadedObjectDataProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (alreadyLoadedObjectData));
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToNotYetLoadedObject ()
    {
      var parentObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      _parentTransactionContextMock
          .Expect (mock => mock.ResolveRelatedObject (_virtualObjectRelationEndPointID))
          .Return (parentObject);
      _alreadyLoadedObjectDataProviderMock
          .Expect (mock => mock.GetLoadedObject (parentObject.ID))
          .Return (null);

      var parentDataContainer = CreateChangedDataContainer (parentObject.ID, 4711, _fileNamePropertyDefinition, "Hugo");
      CheckDataContainer (parentDataContainer, parentObject.ID, 4711, StateType.Changed, _fileNamePropertyDefinition, "Hugo", "", true);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (parentObject.ID, true))
          .Return (parentDataContainer);

      var result = _persistenceStrategy.ResolveObjectRelationData (_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock);

      _parentTransactionContextMock.VerifyAllExpectations ();
      _alreadyLoadedObjectDataProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<FreshlyLoadedObjectData> ());
      Assert.That (result.ObjectID, Is.EqualTo (parentObject.ID));
      
      var dataContainer = ((FreshlyLoadedObjectData) result).FreshlyLoadedDataContainer;
      CheckDataContainer (dataContainer, parentObject.ID, 4711, StateType.Unchanged, _fileNamePropertyDefinition, "Hugo", "Hugo", false);
    }

    [Test]
    public void ResolveObjectRelationData_ResolvesToNull ()
    {
      _parentTransactionContextMock
          .Expect (mock => mock.ResolveRelatedObject (_virtualObjectRelationEndPointID))
          .Return (null);

      var result = _persistenceStrategy.ResolveObjectRelationData (_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<NullLoadedObjectData> ());
    }

    [Test]
    public void ResolveObjectRelationData_NonVirtualObjectEndPoint ()
    {
      Assert.That (
          () => _persistenceStrategy.ResolveObjectRelationData (_collectionEndPointID, _alreadyLoadedObjectDataProviderMock),
          Throws.ArgumentException.With.Message.EqualTo (
            "ResolveObjectRelationData can only be called for virtual object end points.\r\nParameter name: relationEndPointID"));
      Assert.That (
          () => _persistenceStrategy.ResolveObjectRelationData (_nonVirtualEndPointID, _alreadyLoadedObjectDataProviderMock),
          Throws.ArgumentException.With.Message.EqualTo (
            "ResolveObjectRelationData can only be called for virtual object end points.\r\nParameter name: relationEndPointID"));
    }

    [Test]
    public void ResolveCollectionRelationData ()
    {
      var parentObjects =
          new DomainObject[]
          {
              DomainObjectMother.CreateFakeObject<OrderItem>(),
              DomainObjectMother.CreateFakeObject<OrderItem>()
          };
      _parentTransactionContextMock
          .Expect (mock => mock.ResolveRelatedObjects (_collectionEndPointID))
          .Return (parentObjects.AsOneTime());

      var alreadyLoadedObjectData = MockRepository.GenerateStub<ILoadedObjectData> ();
      _alreadyLoadedObjectDataProviderMock
          .Expect (mock => mock.GetLoadedObject (parentObjects[0].ID))
          .Return (alreadyLoadedObjectData);
      _alreadyLoadedObjectDataProviderMock
          .Expect (mock => mock.GetLoadedObject (parentObjects[1].ID))
          .Return (null);

      var parentDataContainer = CreateChangedDataContainer (parentObjects[1].ID, 4711, _productPropertyDefinition, "Keyboard");
      CheckDataContainer (parentDataContainer, parentObjects[1].ID, 4711, StateType.Changed, _productPropertyDefinition, "Keyboard", "", true);
      _parentTransactionContextMock
          .Expect (mock => mock.GetDataContainerWithLazyLoad (parentObjects[1].ID, true))
          .Return (parentDataContainer);

      var result = _persistenceStrategy.ResolveCollectionRelationData (_collectionEndPointID, _alreadyLoadedObjectDataProviderMock).ToList();

      _parentTransactionContextMock.VerifyAllExpectations ();
      _alreadyLoadedObjectDataProviderMock.VerifyAllExpectations ();
      Assert.That (result[0], Is.SameAs (alreadyLoadedObjectData));
      Assert.That (result[1], Is.TypeOf<FreshlyLoadedObjectData> ());
      var dataContainer = ((FreshlyLoadedObjectData) result[1]).FreshlyLoadedDataContainer;
      CheckDataContainer (dataContainer, parentDataContainer.ID, 4711, StateType.Unchanged, _productPropertyDefinition, "Keyboard", "Keyboard", false);
    }

    [Test]
    public void ResolveCollectionRelationData_NonCollectionEndPoint ()
    {
      Assert.That (
          () => _persistenceStrategy.ResolveCollectionRelationData (_virtualObjectRelationEndPointID, _alreadyLoadedObjectDataProviderMock),
          Throws.ArgumentException.With.Message.EqualTo (
            "ResolveCollectionRelationData can only be called for CollectionEndPoints.\r\nParameter name: relationEndPointID"));
      Assert.That (
          () => _persistenceStrategy.ResolveCollectionRelationData (_nonVirtualEndPointID, _alreadyLoadedObjectDataProviderMock),
          Throws.ArgumentException.With.Message.EqualTo (
            "ResolveCollectionRelationData can only be called for CollectionEndPoints.\r\nParameter name: relationEndPointID"));
    }

    [Test]
    public void ExecuteCustomQuery ()
    {
      var fakeResult = new IQueryResultRow[0];
      
      _parentTransactionContextMock
          .Expect (mock => mock.ExecuteCustomQuery (_queryStub))
          .Return (fakeResult);

      var result = _persistenceStrategy.ExecuteCustomQuery (_queryStub);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      var fakeResult = new object();

      _parentTransactionContextMock
          .Expect (mock => mock.ExecuteScalarQuery (_queryStub))
          .Return (fakeResult);

      var result = _persistenceStrategy.ExecuteScalarQuery (_queryStub);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void PersistData_NewDataContainer ()
    {
      var instance = DomainObjectMother.CreateFakeObject<Order> ();
      var dataContainer = DataContainer.CreateNew (instance.ID);
      SetPropertyValue (dataContainer, typeof (Order), "OrderNumber", 12);
      dataContainer.SetDomainObject (instance);

      var persistableData = new PersistableData (instance, StateType.New, dataContainer, new IRelationEndPoint[0]);

      _parentTransactionContextMock.Stub (mock => mock.IsInvalid (instance.ID)).Return (true);
      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);
      _parentTransactionContextMock.Stub (stub => stub.GetDataContainerWithoutLoading (instance.ID)).Return (null);

      using (_unlockedParentTransactionContextMock.GetMockRepository ().Ordered ())
      {
        _unlockedParentTransactionContextMock.Expect (mock => mock.MarkNotInvalid (instance.ID));
        _unlockedParentTransactionContextMock
            .Expect (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var dc = (DataContainer) mi.Arguments[0];
                  Assert.That (dc.ID, Is.EqualTo (instance.ID));
                  Assert.That (dc.State, Is.EqualTo (StateType.New));
                  Assert.That (dc.HasDomainObject, Is.True);
                  Assert.That (dc.DomainObject, Is.SameAs (instance));
                  Assert.That (GetPropertyValue (dc, typeof (Order), "OrderNumber"), Is.EqualTo (12));
                }
            );
        _unlockedParentTransactionContextMock.Expect (mock => mock.Dispose());
      }

      _persistenceStrategy.PersistData (Array.AsReadOnly (new[] { persistableData }));

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
    }

    private void CheckDataContainer (
        DataContainer dataContainer,
        ObjectID expectedID,
        int expectedTimestamp,
        StateType expectedState,
        PropertyDefinition propertyDefinition,
        object expectedCurrentPropertyValue,
        object expectedOriginalPropertyValue,
        bool expectedHasPropertyValueBeenTouched)
    {
      Assert.That (dataContainer.ID, Is.EqualTo (expectedID));
      Assert.That (dataContainer.Timestamp, Is.EqualTo (expectedTimestamp));
      Assert.That (dataContainer.State, Is.EqualTo (expectedState));
      Assert.That (dataContainer.GetValue (propertyDefinition, ValueAccess.Current), Is.EqualTo (expectedCurrentPropertyValue));
      Assert.That (dataContainer.GetValue (propertyDefinition, ValueAccess.Original), Is.EqualTo (expectedOriginalPropertyValue));
      Assert.That (dataContainer.HasValueBeenTouched (propertyDefinition), Is.EqualTo (expectedHasPropertyValueBeenTouched));
    }

    private DataContainer CreateChangedDataContainer (
        ObjectID objectID, int timestamp, PropertyDefinition propertyDefinition, object currentPropertyValue)
    {
      var parentDataContainer = DataContainerObjectMother.CreateExisting (objectID);
      parentDataContainer.SetTimestamp (timestamp);
      parentDataContainer.SetValue (propertyDefinition, currentPropertyValue);
      return parentDataContainer;
    }
  }
}