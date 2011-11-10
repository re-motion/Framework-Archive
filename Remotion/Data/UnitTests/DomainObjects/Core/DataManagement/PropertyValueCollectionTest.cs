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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class PropertyValueCollectionTest : StandardMappingTest
  {
    private PropertyValueCollection _collection;

    public override void SetUp ()
    {
      base.SetUp ();

      _collection = new PropertyValueCollection ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Property 'DoesNotExist' does not exist.\r\nParameter name: propertyName")]
    public void NonExistingPropertyName ()
    {
      _collection.Add (CreatePropertyValue ("PropertyName 1"));
      _collection.Add (CreatePropertyValue ("PropertyName 2"));

      Dev.Null = _collection["DoesNotExist"];
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Property 'PropertyName' already exists in collection.\r\nParameter name: value")]
    public void DuplicatePropertyNames ()
    {
      _collection.Add (CreatePropertyValue ("PropertyName"));
      _collection.Add (CreatePropertyValue ("PropertyName"));
    }

    [Test]
    public void ContainsPropertyValueTrue ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName");

      _collection.Add (value);

      Assert.IsTrue (_collection.Contains (value));
    }

    [Test]
    public void ContainsPropertyValueFalse ()
    {
      PropertyValue value = CreatePropertyValue ("PropertyName");
      _collection.Add (value);

      PropertyValue copy = CreatePropertyValue ("PropertyName");

      Assert.IsFalse (_collection.Contains (copy));

    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullPropertyValue ()
    {
      _collection.Contains ((PropertyValue) null);
    }

    private PropertyValue CreatePropertyValue (string name)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();
      var definition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (classDefinition, name);
      return new PropertyValue (definition, definition.DefaultValue);
    }
  }
}
