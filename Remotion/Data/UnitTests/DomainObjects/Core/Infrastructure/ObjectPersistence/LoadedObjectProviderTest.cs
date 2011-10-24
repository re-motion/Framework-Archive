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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectProviderTest : StandardMappingTest
  {
    private IDataContainerProvider _dataContainerProviderMock;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;

    private LoadedObjectProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataContainerProviderMock = MockRepository.GenerateStrictMock<IDataContainerProvider>();
      _invalidDomainObjectManagerMock = MockRepository.GenerateStrictMock<IInvalidDomainObjectManager>();
      _provider = new LoadedObjectProvider (_dataContainerProviderMock, _invalidDomainObjectManagerMock);
    }

    [Test]
    public void GetLoadedObject_Known ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject (DomainObjectMother.CreateFakeObject<Order> (dataContainer.ID));
      DataContainerTestHelper.SetClientTransaction (dataContainer, ClientTransaction.CreateRootTransaction());

      _invalidDomainObjectManagerMock
          .Stub (mock => mock.IsInvalid (DomainObjectIDs.Order1))
          .Return (false);
      _invalidDomainObjectManagerMock.Replay ();

      _dataContainerProviderMock
          .Expect (mock => mock.GetDataContainerWithoutLoading (DomainObjectIDs.Order1))
          .Return (dataContainer);
      _dataContainerProviderMock.Replay ();

      var loadedObject = _provider.GetLoadedObject (DomainObjectIDs.Order1);

      _dataContainerProviderMock.VerifyAllExpectations();
      Assert.That (
          loadedObject, 
          Is.TypeOf<AlreadyExistingLoadedObject> ()
            .With.Property ((AlreadyExistingLoadedObject obj) => obj.ExistingDataContainer).SameAs (dataContainer));
    }

    [Test]
    public void GetLoadedObject_Unknown ()
    {
      _invalidDomainObjectManagerMock
        .Stub (mock => mock.IsInvalid (DomainObjectIDs.Order1))
        .Return (false);
      _invalidDomainObjectManagerMock.Replay ();

      _dataContainerProviderMock
          .Expect (mock => mock.GetDataContainerWithoutLoading (DomainObjectIDs.Order1))
          .Return (null);
      _dataContainerProviderMock.Replay ();

      var loadedObject = _provider.GetLoadedObject (DomainObjectIDs.Order1);

      _dataContainerProviderMock.VerifyAllExpectations ();
      Assert.That (loadedObject, Is.Null);
    }

    [Test]
    public void GetLoadedObject_Invalid ()
    {
      var invalidObjectReference = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);

      _invalidDomainObjectManagerMock
          .Expect (mock => mock.IsInvalid (DomainObjectIDs.Order1))
          .Return (true);
      _invalidDomainObjectManagerMock
          .Expect (mock => mock.GetInvalidObjectReference (DomainObjectIDs.Order1))
          .Return (invalidObjectReference);
      _invalidDomainObjectManagerMock.Replay();
      _dataContainerProviderMock.Replay();

      var loadedObject = _provider.GetLoadedObject (DomainObjectIDs.Order1);

      _dataContainerProviderMock.AssertWasNotCalled (mock => mock.GetDataContainerWithoutLoading (Arg<ObjectID>.Is.Anything));
      _invalidDomainObjectManagerMock.VerifyAllExpectations();

      Assert.That (
          loadedObject, 
          Is.TypeOf<InvalidLoadedObject>().With.Property ((InvalidLoadedObject obj) => obj.InvalidObjectReference).SameAs (invalidObjectReference));
    }

    [Test]
    public void Serializable ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var provider = new LoadedObjectProvider (
          ClientTransactionTestHelper.GetDataManager (clientTransaction),
          ClientTransactionTestHelper.GetInvalidDomainObjectManager (clientTransaction));

      var deserializedInstance = Serializer.SerializeAndDeserialize (provider);

      Assert.That (deserializedInstance.DataContainerProvider, Is.Not.Null);
      Assert.That (deserializedInstance.InvalidDomainObjectManager, Is.Not.Null);
    }
  }
}