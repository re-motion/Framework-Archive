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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains
{
  [TestFixture]
  public class DomainObjectMixinHookTest : StandardMappingTest
  {
    private IObjectID<DomainObject> _objectID;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectID = ObjectID.Create(typeof (HookedTargetClass), Guid.NewGuid ());
    }

    [Test]
    public void OnDomainObjectLoaded ()
    {
      var tx = CreateTransactionWithStubbedLoading (_objectID);

      var mixinInstance = new HookedDomainObjectMixin();

      Assert.That (mixinInstance.OnLoadedCalled, Is.False);
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.False);

      using (new MixedObjectInstantiationScope (mixinInstance))
      {
        LifetimeService.GetObject (tx, _objectID, false);
      }

      Assert.That (mixinInstance.OnLoadedCalled, Is.True);
      Assert.That (mixinInstance.OnLoadedLoadMode, Is.EqualTo (LoadMode.WholeDomainObjectInitialized));
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void OnDomainObjectLoadedAfterEnlist ()
    {
      var tx = CreateTransactionWithStubbedLoading (_objectID);

      var mixinInstance = new HookedDomainObjectMixin();

      Assert.That (mixinInstance.OnLoadedCalled, Is.False);
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.False);

      HookedTargetClass instance;
      using (new MixedObjectInstantiationScope (mixinInstance))
      {
        instance = (HookedTargetClass) LifetimeService.GetObject (tx, _objectID, false);
      }

      mixinInstance.OnLoadedCalled = false;
      mixinInstance.OnLoadedCount = 0;

      ClientTransaction newTransaction = CreateTransactionWithStubbedLoading (_objectID);
      newTransaction.EnlistDomainObject (instance);

      Assert.That (mixinInstance.OnLoadedCalled, Is.False);

      using (newTransaction.EnterDiscardingScope())
      {
        ++instance.Property;
      }

      Assert.That (mixinInstance.OnLoadedCalled, Is.True);
      Assert.That (mixinInstance.OnLoadedLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
      Assert.That (mixinInstance.OnLoadedCount, Is.EqualTo (1));
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void OnDomainObjectLoadedInSubTransaction ()
    {
      var tx = CreateTransactionWithStubbedLoading (_objectID);

      var mixinInstance = new HookedDomainObjectMixin ();

      Assert.That (mixinInstance.OnLoadedCalled, Is.False);
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.False);

      using (new MixedObjectInstantiationScope (mixinInstance))
      {
        var subTx = tx.CreateSubTransaction ();
        LifetimeService.GetObject (subTx, _objectID, false);
        subTx.Discard ();
      }

      Assert.That (mixinInstance.OnLoadedCalled, Is.True);
      Assert.That (mixinInstance.OnLoadedCount, Is.EqualTo (2));
      Assert.That (mixinInstance.OnLoadedLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
    }

    [Test]
    public void OnDomainObjectLoadedInParentAndSubTransaction ()
    {
      var tx = CreateTransactionWithStubbedLoading (_objectID);

      var mixinInstance = new HookedDomainObjectMixin ();

      Assert.That (mixinInstance.OnLoadedCalled, Is.False);
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.False);

      using (new MixedObjectInstantiationScope (mixinInstance))
      {
        LifetimeService.GetObject (tx, _objectID, false);

        Assert.That (mixinInstance.OnLoadedCalled, Is.True);
        Assert.That (mixinInstance.OnLoadedCount, Is.EqualTo (1));
        Assert.That (mixinInstance.OnLoadedLoadMode, Is.EqualTo (LoadMode.WholeDomainObjectInitialized));
        Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
        Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCount, Is.EqualTo (1));

        using (new MixedObjectInstantiationScope (mixinInstance))
        {
          var subTx = tx.CreateSubTransaction ();
          LifetimeService.GetObject (subTx, _objectID, false);
          subTx.Discard ();
        }
      }

      Assert.That (mixinInstance.OnLoadedCount, Is.EqualTo (2));
      Assert.That (mixinInstance.OnLoadedLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCount, Is.EqualTo (1));
    }

    [Test]
    public void OnDomainObjectCreated ()
    {
      var mixinInstance = new HookedDomainObjectMixin();

      Assert.That (mixinInstance.OnLoadedCalled, Is.False);
      Assert.That (mixinInstance.OnCreatedCalled, Is.False);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.False);

      using (new MixedObjectInstantiationScope (mixinInstance))
      {
        var tx = ClientTransaction.CreateRootTransaction();
        LifetimeService.NewObject (tx, typeof (HookedTargetClass), ParamList.Empty);
      }

      Assert.That (mixinInstance.OnLoadedCalled, Is.False);
      Assert.That (mixinInstance.OnCreatedCalled, Is.True);
      Assert.That (mixinInstance.OnDomainObjectReferenceInitializingCalled, Is.True);
    }

    private ClientTransaction CreateTransactionWithStubbedLoading (IObjectID<DomainObject> id)
    {
      return CreateTransactionWithStubbedLoading (DataContainer.CreateForExisting (id, null, pd => pd.DefaultValue));
    }

    private ClientTransaction CreateTransactionWithStubbedLoading (DataContainer loadableDataContainer)
    {
      var persistenceStrategyStub = MockRepository.GenerateStub<IFetchEnabledPersistenceStrategy>();
      persistenceStrategyStub.Stub (stub => stub.LoadObjectData (loadableDataContainer.ID)).Return (new FreshlyLoadedObjectData (loadableDataContainer));
      persistenceStrategyStub
          .Stub (stub => stub.LoadObjectData (Arg<IEnumerable<IObjectID<DomainObject>>>.List.Equal (new[] { loadableDataContainer.ID })))
          .Return (new[] { new FreshlyLoadedObjectData (loadableDataContainer)});
      return ClientTransactionObjectMother.CreateTransactionWithPersistenceStrategy<ClientTransaction> (persistenceStrategyStub);
    }
  }
}