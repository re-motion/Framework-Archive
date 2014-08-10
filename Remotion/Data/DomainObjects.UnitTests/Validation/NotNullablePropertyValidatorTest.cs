﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class NotNullablePropertyValidatorTest : StandardMappingTest
  {
     private NotNullablePropertyValidator _validator;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _validator = new NotNullablePropertyValidator();
    }

    [Test]
    public void Validate_PropertyIsNullable_AndPropertyValueIsNull_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataItem = CreatePersistableData (StateType.New, domainObject);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), "Not Null");
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (Person), "AssociatedCustomerCompany"), null);

      Assert.That (() => _validator.Validate (dataItem), Throws.Nothing);
    }

    [Test]
    public void Validate_DoesNotRaisePropertyValueReadEvents ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataItem = CreatePersistableData (StateType.New, domainObject);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), "Not Null");
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (Person), "AssociatedCustomerCompany"), null);
      var eventListenerStub = MockRepository.GenerateStub<IDataContainerEventListener>();
      dataItem.DataContainer.SetEventListener (eventListenerStub);

      _validator.Validate (dataItem);

      eventListenerStub.AssertWasNotCalled (_ => _.PropertyValueReading (null, null, ValueAccess.Current), mo => mo.IgnoreArguments());
    }

    [Test]
    public void Validate_PropertyIsNotNullable_AndPropertyValueIsNull_ThrowsException ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataItem = CreatePersistableData (StateType.New, domainObject);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), null);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (Person), "AssociatedCustomerCompany"), DomainObjectIDs.Customer1);

      Assert.That (
          () => _validator.Validate (dataItem),
          Throws.TypeOf<PropertyValueNotSetException>().With.Message.Matches (
              @"Not-nullable property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Person\.Name' of domain object "
              + @"'Person|.*|System\.Guid' cannot be null."));
    }

    [Test]
    public void Validate_IgnoresDeletedObjects ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataItem = CreatePersistableData (StateType.Deleted, domainObject);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), null);

      Assert.That (() => _validator.Validate (dataItem), Throws.Nothing);
    }

    [Test]
    public void Validate_IntegrationTest_PropertyOk ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var person = Person.NewObject ();
        person.Name = "Not Null";

        var persistableData = PersistableDataObjectMother.Create (ClientTransaction.Current, person);
        Assert.That (() => _validator.Validate (persistableData), Throws.Nothing);
      }
    }

    [Test]
    public void Validate_IntegrationTest_PropertyNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope())
      {
        var person = Person.NewObject ();
        person.Name = null;

        var persistableData = PersistableDataObjectMother.Create (ClientTransaction.Current, person);
        Assert.That (
            () => _validator.Validate (persistableData),
            Throws.TypeOf<PropertyValueNotSetException>().With.Message.Matches (
              @"Not-nullable property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Person\.Name' of domain object "
              + @"'Person|.*|System\.Guid' cannot be null."));
      }
    }

    private PersistableData CreatePersistableData (StateType domainObjectState, DomainObject domainObject)
    {
      var dataContainer = DataContainer.CreateNew (domainObject.ID);
      return new PersistableData (domainObject, domainObjectState, dataContainer, Enumerable.Empty<IRelationEndPoint>());
    }
    
  }
}