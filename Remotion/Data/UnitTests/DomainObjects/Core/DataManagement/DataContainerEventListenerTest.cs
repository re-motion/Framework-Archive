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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataContainerEventListenerTest : ForwardingEventListenerTestBase<DataContainerEventListener>
  {
    private DataContainerEventListener _eventListener;

    private DomainObject _domainObject;
    private DataContainer _dataContainer;
    private PropertyDefinition _propertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _eventListener = new DataContainerEventListener (EventSinkWithMock);

      _domainObject = DomainObjectMother.CreateFakeObject();
      _dataContainer = DataContainerObjectMother.CreateWithDomainObject (_domainObject);
      _propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
    }

    protected override DataContainerEventListener EventListener
    {
      get { return _eventListener; }
    }

    [Test]
    public void PropertyValueReading ()
    {
      CheckEventDelegation (
          l => l.PropertyValueReading (_dataContainer, _propertyDefinition, ValueAccess.Original), 
          (tx, mock) => mock.PropertyValueReading (tx, _domainObject, _propertyDefinition, ValueAccess.Original));
    }

    [Test]
    public void PropertyValueRead ()
    {
      CheckEventDelegation (
          l => l.PropertyValueRead (_dataContainer, _propertyDefinition, "value", ValueAccess.Original),
          (tx, mock) => mock.PropertyValueRead (tx, _domainObject, _propertyDefinition, "value", ValueAccess.Original));
    }

    [Test]
    public void PropertyValueChanging ()
    {
      CheckEventDelegation (
          l => l.PropertyValueChanging (_dataContainer, _propertyDefinition, "oldValue", "newValue"),
          (tx, mock) => mock.PropertyValueChanging (tx, _domainObject, _propertyDefinition, "oldValue", "newValue"));
    }

    [Test]
    public void PropertyValueChanged ()
    {
      CheckEventDelegation (
          l => l.PropertyValueChanged (_dataContainer, _propertyDefinition, "oldValue", "newValue"),
          (tx, mock) => mock.PropertyValueChanged (tx, _domainObject, _propertyDefinition, "oldValue", "newValue"));
    }

    [Test]
    public void StateUpdated ()
    {
      CheckEventDelegation (
          l => l.StateUpdated (_dataContainer, StateType.New),
          (tx, mock) => mock.DataContainerStateUpdated (tx, _dataContainer, StateType.New));
    }
    
    [Test]
    public void Serializable ()
    {
      var instance = new DataContainerEventListener (new SerializableClientTransactionEventSinkFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.EventSink, Is.Not.Null);
    }
  }
}